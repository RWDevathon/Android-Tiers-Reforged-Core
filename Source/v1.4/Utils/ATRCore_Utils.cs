using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Linq;

namespace ATReforged
{
    public static class ATRCore_Utils
    {
        // Cached pawn that represents a blank pawn to be used for various duplication and modification tasks.
        private static Pawn blankPawn;

        // GENERAL UTILITIES
        // Return a new Gender for a mechanical pawn, based on settings. This should only be called for androids.
        public static Gender GenerateGender(PawnKindDef pawnKind)
        {
            // If androids are not allowed to have genders by setting, then set to none.
            if (!ATReforgedCore_Settings.androidsHaveGenders)
                return Gender.None;

            // If androids don't pick their own gender by setting, then set it to the one players selected in settings.
            if (!ATReforgedCore_Settings.androidsPickGenders)
                return ATReforgedCore_Settings.androidsFixedGender;

            // If androids pick their gender, then randomly select a gender based on the matching setting.
            if (Rand.Chance(ATReforgedCore_Settings.androidsGenderRatio))
                return Gender.Male;
            // If it did not randomly select male, then it randomly selected female.
            else
                return Gender.Female;
        }

        public static bool IsConsideredMechanicalAndroid(Pawn pawn)
        {
            return IsConsideredMechanicalAndroid(pawn.def);
        }

        public static bool IsConsideredMechanicalAndroid(ThingDef thingDef)
        {
            return cachedAndroidRaces.Contains(thingDef);
        }

        /* === HEALTH UTILITIES === */
        
        // Misc
        // Get a cached Blank pawn (to avoid having to create a new pawn whenever an android is made blank)
        public static Pawn GetBlank()
        {
            if (blankPawn != null)
            {
                return blankPawn;
            }

            // Create the Blank pawn that will be used for all blank androids
            PawnGenerationRequest request = new PawnGenerationRequest(Faction.OfPlayer.def.basicMemberKind, null, PawnGenerationContext.PlayerStarter, canGeneratePawnRelations: false, forceNoIdeo: true, forceBaselinerChance: 1, colonistRelationChanceFactor: 0f, forceGenerateNewPawn: true, fixedGender: Gender.None);
            Pawn blankMechanical = PawnGenerator.GeneratePawn(request);
            blankMechanical.story.Childhood = ATR_BackstoryDefOf.ATR_MechChildhoodFreshBlank;
            blankMechanical.story.Adulthood = ATR_BackstoryDefOf.ATR_MechAdulthoodBlank;
            blankMechanical.story.traits.allTraits.Clear();
            blankMechanical.skills.Notify_SkillDisablesChanged();
            blankMechanical.skills.skills.ForEach(delegate (SkillRecord record)
            {
                record.passion = 0;
                record.Level = 0;
                record.xpSinceLastLevel = 0;
                record.xpSinceMidnight = 0;
            });
            blankMechanical.workSettings.EnableAndInitializeIfNotAlreadyInitialized();
            blankMechanical.workSettings.DisableAll();
            blankMechanical.playerSettings = new Pawn_PlayerSettings(blankMechanical)
            {
                AreaRestriction = null,
                hostilityResponse = HostilityResponseMode.Flee
            };
            if (ModsConfig.BiotechActive)
            {
                for (int i = blankMechanical.genes.GenesListForReading.Count - 1; i >= 0; i--)
                {
                    blankMechanical.genes.RemoveGene(blankMechanical.genes.GenesListForReading[i]);
                }
            }
            if (blankMechanical.ideo != null)
            {
                blankMechanical.ideo.SetIdeo(null);
            }
            if (blankMechanical.timetable == null)
                blankMechanical.timetable = new Pawn_TimetableTracker(blankMechanical);
            if (blankMechanical.playerSettings == null)
                blankMechanical.playerSettings = new Pawn_PlayerSettings(blankMechanical);
            if (blankMechanical.foodRestriction == null)
                blankMechanical.foodRestriction = new Pawn_FoodRestrictionTracker(blankMechanical);
            if (blankMechanical.drugs == null)
                blankMechanical.drugs = new Pawn_DrugPolicyTracker(blankMechanical);
            if (blankMechanical.outfits == null)
                blankMechanical.outfits = new Pawn_OutfitTracker(blankMechanical);
            blankMechanical.Name = new NameTriple("ATR_BlankPawnFirstName".Translate(), "ATR_BlankPawnNickname".Translate(), "ATR_BlankPawnLastName".Translate());
            blankPawn = blankMechanical;
            return blankPawn;
        }

        public static ATR_GameComponent gameComp;

        // Duplicate the source pawn into the destination pawn. If overwriteAsDeath is true, then it is considered murdering the destination pawn.
        // if isTethered is true, then the duplicated pawn will actually share the class with the source so changing one will affect the other automatically.
        public static void Duplicate(Pawn source, Pawn dest, bool overwriteAsDeath=true, bool isTethered = true)
        {
            try
            {
                DuplicateStory(ref source, ref dest);

                DuplicateSkills(source, dest, isTethered);

                // If this duplication is considered to be killing a sapient individual, then handle some relations before they're duplicated.
                if (overwriteAsDeath)
                {
                    PawnDiedOrDownedThoughtsUtility.TryGiveThoughts(dest, null, PawnDiedOrDownedThoughtsKind.Died);
                    Pawn spouse = dest.relations?.GetFirstDirectRelationPawn(PawnRelationDefOf.Spouse);
                    if (spouse != null && !spouse.Dead && spouse.needs.mood != null)
                    {
                        MemoryThoughtHandler memories = spouse.needs.mood.thoughts.memories;
                        memories.RemoveMemoriesOfDef(ThoughtDefOf.GotMarried);
                        memories.RemoveMemoriesOfDef(ThoughtDefOf.HoneymoonPhase);
                    }
                    Traverse.Create(dest.relations).Method("AffectBondedAnimalsOnMyDeath").GetValue();
                    dest.health.NotifyPlayerOfKilled(null, null, null);
                    dest.relations.ClearAllRelations();
                }

                // Duplicate relations.
                DuplicateRelations(source, dest, isTethered);

                // If Ideology dlc is active, duplicate pawn ideology into destination.
                if (ModsConfig.IdeologyActive)
                {
                    DuplicateIdeology(source, dest, isTethered);
                }

                // If Royalty dlc is active, then handle it. Royalty is non-transferable, but it should be checked for the other details that have been duplicated.
                if (ModsConfig.RoyaltyActive)
                {
                    DuplicateRoyalty(source, dest, isTethered);
                }

                // Duplicate faction. No difference if tethered or not.
                if (source.Faction != dest.Faction)
                    dest.SetFaction(source.Faction);

                // Duplicate source needs into destination. This is not tetherable.
                DuplicateNeeds(source, dest);

                // Only duplicate source settings for player pawns as foreign pawns don't need them. Can not be tethered as otherwise pawns would be forced to have same work/time/role settings.
                if (source.Faction != null && dest.Faction != null && source.Faction.IsPlayer && dest.Faction.IsPlayer)
                {
                    DuplicatePlayerSettings(source, dest);
                }

                // Duplicate source name into destination.
                NameTriple sourceName = (NameTriple)source.Name;
                dest.Name = new NameTriple(sourceName.First, sourceName.Nick, sourceName.Last);

                dest.Drawer.renderer.graphics.ResolveAllGraphics();
            }
            catch(Exception e)
            {
                Log.Error("[ATR] Utils.Duplicate: Error occurred duplicating " + source + " into " + dest + ". This will have severe consequences. " + e.Message + e.StackTrace);
            }
        }

        // Duplicate all appropriate details from the StoryTracker of the source into the destination.
        public static void DuplicateStory(ref Pawn source, ref Pawn dest)
        {
            if (source.story == null || dest.story == null)
            {
                Log.Warning("[ATR] A Storytracker for a duplicate operation was null. Destination story unchanged. This will have no further effects.");
                return;
            }

            try
            {
                // Clear all destination traits first to avoid issues. Only remove traits that are unspecific to genes.
                foreach (Trait trait in dest.story.traits.allTraits.ToList().Where(trait => trait.sourceGene == null))
                {
                    dest.story.traits.RemoveTrait(trait);
                }

                // Add all source traits to the destination. Only add traits that are unspecific to genes.
                foreach (Trait trait in source.story.traits?.allTraits.Where(trait => trait.sourceGene == null))
                {
                    dest.story.traits.GainTrait(new Trait(trait.def, trait.Degree, true));
                }

                // Copy some backstory related details, and double check work types and skill modifiers.
                dest.story.Childhood = source.story.Childhood;
                dest.story.Adulthood = source.story.Adulthood;
                dest.story.title = source.story.title;
                dest.story.favoriteColor = source.story.favoriteColor;
                dest.Notify_DisabledWorkTypesChanged();
                dest.skills.Notify_SkillDisablesChanged();
            }
            catch (Exception exception)
            {
                Log.Warning("[ATR] An unexpected error occurred during story duplication between " + source + " " + dest + ". The destination StoryTracker may be left unstable!" + exception.Message + exception.StackTrace);
            }
        }

        // Duplicate ideology details from the source to the destination.
        public static void DuplicateIdeology(Pawn source, Pawn dest, bool isTethered)
        {
            try
            {
                // If source ideology is null, then destination's ideology should also be null. Vanilla handles null ideologies relatively gracefully.
                if (source.ideo == null)
                {
                    dest.ideo = null;
                }
                // If untethered, copy the details of the ideology over, as a separate copy.
                else if (!isTethered)
                {
                    dest.ideo = new Pawn_IdeoTracker(dest);
                    dest.ideo.SetIdeo(source.Ideo);
                    dest.ideo.OffsetCertainty(source.ideo.Certainty - dest.ideo.Certainty);
                    dest.ideo.joinTick = source.ideo.joinTick;
                }
                // If tethered, the destination and source will share a single IdeologyTracker.
                else
                {
                    dest.ideo = source.ideo;
                }
            }
            catch (Exception exception)
            {
                Log.Warning("[ATR] An unexpected error occurred during ideology duplication between " + source + " " + dest + ". The destination IdeoTracker may be left unstable!" + exception.Message + exception.StackTrace);
            }
        }

        // Royalty status can not actually be duplicated, but duplicating a pawn should still handle cases around royal abilities/details.
        public static void DuplicateRoyalty(Pawn source, Pawn dest, bool isTethered)
        {
            try
            {
                if (source.royalty != null)
                {
                    source.royalty.UpdateAvailableAbilities();
                    if (source.needs != null)
                        source.needs.AddOrRemoveNeedsAsAppropriate();
                    source.abilities.Notify_TemporaryAbilitiesChanged();
                }
                if (dest.royalty != null)
                {
                    dest.royalty.UpdateAvailableAbilities();
                    if (dest.needs != null)
                        dest.needs.AddOrRemoveNeedsAsAppropriate();
                    dest.abilities.Notify_TemporaryAbilitiesChanged();
                }
            }
            catch (Exception exception)
            {
                Log.Warning("[ATR] An unexpected error occurred during royalty duplication between " + source + " " + dest + ". No further issues are anticipated." + exception.Message + exception.StackTrace);
            }
        }

        // Duplicate all skill levels, xp gains, and passions into the destination.
        public static void DuplicateSkills(Pawn source, Pawn dest, bool isTethered)
        {
            try
            {
                // If untethered, create a copy of the source SkillTracker for the destination to use.
                // Explicitly create a new skill tracker to avoid any issues with tethered skill trackers.
                if (!isTethered)
                {
                    Pawn_SkillTracker destSkills = new Pawn_SkillTracker(dest);
                    foreach (SkillDef skillDef in DefDatabase<SkillDef>.AllDefsListForReading)
                    {
                        SkillRecord newSkill = destSkills.GetSkill(skillDef);
                        SkillRecord sourceSkill = source.skills.GetSkill(skillDef);
                        newSkill.Level = sourceSkill.Level;
                        newSkill.passion = sourceSkill.passion;
                        newSkill.xpSinceLastLevel = sourceSkill.xpSinceLastLevel;
                        newSkill.xpSinceMidnight = sourceSkill.xpSinceMidnight;
                    }
                    dest.skills = destSkills;
                }
                // If tethered, the destination and source will share their skill tracker directly.
                else
                {
                    dest.skills = source.skills;
                }
            }
            catch (Exception exception)
            {
                Log.Warning("[ATR] An unexpected error occurred during skill duplication between " + source + " " + dest + ". The destination SkillTracker may be left unstable!" + exception.Message + exception.StackTrace);
            }
        }

        // Duplicate relations from the source to the destination. This should also affect other pawn relations, and any animals involved.
        public static void DuplicateRelations(Pawn source, Pawn dest, bool isTethered)
        {
            try
            {
                // If untethered, copy all relations that involve the source pawn and apply them to the destination. As animals may have only one master, assign it to the destination.
                if (!isTethered)
                {
                    // Create a new tracker to avoid overwriting anything previously tethered and dealing with old data.
                    Pawn_RelationsTracker destRelations = new Pawn_RelationsTracker(dest);

                    List<Pawn> checkedOtherPawns = new List<Pawn>();
                    // Duplicate all of the source's relations. Ensure that other pawns with relations to the source also have them to the destination.
                    foreach (DirectPawnRelation pawnRelation in source.relations?.DirectRelations?.ToList())
                    {
                        // Ensure that we check the pawn relations for the opposite side only once to avoid doing duplicate relations.
                        if (!checkedOtherPawns.Contains(pawnRelation.otherPawn))
                        {
                            // Iterate through all of the other pawn's relations and copy any they have with the source onto the destination.
                            foreach (DirectPawnRelation otherPawnRelation in pawnRelation.otherPawn.relations?.DirectRelations.ToList())
                            {
                                if (otherPawnRelation.otherPawn == source)
                                {
                                    pawnRelation.otherPawn.relations?.AddDirectRelation(otherPawnRelation.def, dest);
                                }
                            }
                            checkedOtherPawns.Add(pawnRelation.otherPawn);
                        }
                        destRelations.AddDirectRelation(pawnRelation.def, pawnRelation.otherPawn);
                    }

                    destRelations.everSeenByPlayer = true;
                    dest.relations = destRelations;

                    // Transfer animal master status to destination
                    foreach (Map map in Find.Maps)
                    {
                        foreach (Pawn animal in map.mapPawns.SpawnedColonyAnimals)
                        {
                            if (animal.playerSettings == null)
                                continue;

                            if (animal.playerSettings.Master != null && animal.playerSettings.Master == source)
                                animal.playerSettings.Master = dest;
                        }
                    }
                }
                // Tether destination relations to the source.
                else
                {
                    dest.relations = source.relations;
                }
            }
            catch (Exception exception)
            {
                Log.Warning("[ATR] An unexpected error occurred during relation duplication between " + source + " " + dest + ". The destination RelationTracker may be left unstable!" + exception.Message + exception.StackTrace);
            }
        }

        // Duplicate applicable needs from the source to the destination. This includes mood thoughts, memories, and ensuring it updates its needs as appropriate.
        public static void DuplicateNeeds(Pawn source, Pawn dest)
        {
            try
            {
                Pawn_NeedsTracker newNeeds = new Pawn_NeedsTracker(dest);
                if (source.needs?.mood != null)
                {
                    foreach (Thought_Memory memory in source.needs.mood.thoughts.memories.Memories)
                    {
                        newNeeds.mood.thoughts.memories.TryGainMemory(memory, memory.otherPawn);
                    }
                }
                dest.needs = newNeeds;
                dest.needs?.AddOrRemoveNeedsAsAppropriate();
                dest.needs?.mood?.thoughts?.situational?.Notify_SituationalThoughtsDirty();
            }
            catch (Exception exception)
            {
                Log.Warning("[ATR] An unexpected error occurred during need duplication between " + source + " " + dest + ". The destination NeedTracker may be left unstable!" + exception.Message + exception.StackTrace);
            }
        }

        public static void DuplicatePlayerSettings(Pawn source, Pawn dest)
        {
            try
            {
                // Initialize source work settings if not initialized.
                if (source.workSettings == null)
                {
                    source.workSettings = new Pawn_WorkSettings(source);
                }
                source.workSettings.EnableAndInitializeIfNotAlreadyInitialized();

                // Initialize destination work settings if not initialized.
                if (dest.workSettings == null)
                {
                    dest.workSettings = new Pawn_WorkSettings(dest);
                }
                dest.workSettings.EnableAndInitializeIfNotAlreadyInitialized();

                // Apply work settings to destination from the source
                if (source.workSettings != null && source.workSettings.EverWork)
                {
                    foreach (WorkTypeDef workTypeDef in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                    {
                        if (!dest.WorkTypeIsDisabled(workTypeDef))
                            dest.workSettings.SetPriority(workTypeDef, source.workSettings.GetPriority(workTypeDef));
                    }
                }

                // Duplicate source restrictions from into destination.
                for (int i = 0; i != 24; i++)
                {
                    dest.timetable.SetAssignment(i, source.timetable.GetAssignment(i));
                }

                dest.playerSettings = new Pawn_PlayerSettings(dest);
                dest.playerSettings.AreaRestriction = source.playerSettings.AreaRestriction;
                dest.playerSettings.hostilityResponse = source.playerSettings.hostilityResponse;
                dest.outfits = new Pawn_OutfitTracker(dest);
                dest.outfits.CurrentOutfit = source.outfits.CurrentOutfit;
            }
            catch (Exception exception)
            {
                Log.Warning("[ATR] An unexpected error occurred during player setting duplication between " + source + " " + dest + ". The destination PlayerSettings may be left unstable!" + exception.Message + exception.StackTrace);
            }
        }

        // Cached races that are considered androids for establishing correct behavior, cached at startup.
        public static HashSet<ThingDef> cachedAndroidRaces = new HashSet<ThingDef>();
    }
}
