using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using TT.Domain.Statics;

namespace TT.Domain.Models
{
    public class Contribution
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string OwnerMembershipId {get; set;}

        [Display(Name = "What name do you want to be credited with?  Only write the original author's own name here.  Co-contributors should be listed later on.")]
        public string SubmitterName { get; set; }

        [Display(Name = "Other contributors, anyone else you would like to thank for ideas, proofreading, or anything else.")]
        public string AdditionalSubmitterNames { get; set; }

        [Display(Name = "Do you have a website whose URL you'd like to have listed along with your name when being credited?  If so, enter the URL, otherwise leave empty.")]
        public string SubmitterUrl { get; set; }

        // StaticSkill fields
        #region skill
        
        [Display(Name = "Name of spell")]
        public string Skill_FriendlyName { get; set; }

        [Display(Name = "Which form will this spell turn the target into?")]
        public string Skill_FormFriendlyName { get; set; }

        [Display(Name = "Description of this spell")]
        public string Skill_Description { get; set; }

        [Display(Name = "Mana cost (this should always be set to 7")]
        public decimal Skill_ManaCost { get; set; }

        [Display(Name = "Transformation points added by a successful casting of this spell (this should always be set to 10)")]
        public decimal Skill_TFPointsAmount { get; set; }

        [Display(Name = "Target's willpower decrease when hit by this spell (this should always be set to 4.5)")]
        public decimal Skill_HealthDamageAmount { get; set; }

         [Display(Name = "Is this spell learned at a region or specific location?")]
        public string Skill_LearnedAtLocationOrRegion { get; set; }

        [Display(Name = "Where is this spell learned, discovered, or invented?  (If the  desired location does not currently exist, I will probably be able to add it in to the game.)")]
        public string Skill_LearnedAtRegion { get; set; }

        [Display(Name = "Short description of how a character learns, discovers, or invents this spell.")]
        public string Skill_DiscoveryMessage { get; set; }

        [Display(Name = "Is this a spell that a player or bot can learn (from spellbooks, random searches, etc?)")]
        public bool Skill_IsPlayerLearnable { get; set; }

        #endregion

        // Form fields
        #region form

        [Display(Name = "Name of this form")]
        public string Form_FriendlyName { get; set; }

        [Display(Name = "Description of what this form looks and acts like.  If the form is inanimate, write about how it feels for the transformed person to be that new item.  !!! If the form is animate, please do not describe what he or she is wearing, or else keep it to a minimum--focus more on how this person feels and acts. !!!")]
        public string Form_Description { get; set; }

        [Display(Name = "How many transformation energy points are needed to fully transform a target into this form (must be set to 100)")]
        public decimal Form_TFEnergyRequired { get; set; }

         [Display(Name = "Gender of this form.  If the form is inanimate, use the gender of whoever is most likely to own or wear it.")]
        public string Form_Gender { get; set; }

         [Display(Name = "Mobility type")]
        public string Form_MobilityType { get; set; }

         [Display(Name = "What item does this form turn into (if you chose 'inanimate' above)?")]
        public string Form_BecomesItemDbName { get; set; }

         [Display(Name = "What bonuses and penalties does being the form give the player?  (Only if form is not inanimate.)")]
         public string Form_Bonuses { get; set; }


        //public string Form_PortraitUrl { get; set; }

        // 1st person gender netural

         [Display(Name = "1st person 0-20 % transformed, gender neutral")]
        public string Form_TFMessage_20_Percent_1st { get; set; }

         [Display(Name = "1st person 20-40 % transformed, gender neutral")]
        public string Form_TFMessage_40_Percent_1st { get; set; }

         [Display(Name = "1st person 40-60 % transformed, gender neutral")]
        public string Form_TFMessage_60_Percent_1st { get; set; }

         [Display(Name = "1st person 60-80 % transformed, gender neutral")]
        public string Form_TFMessage_80_Percent_1st { get; set; }

         [Display(Name = "1st person 80-99 % transformed, gender neutral")]
        public string Form_TFMessage_100_Percent_1st { get; set; }

         [Display(Name = "1st person 100 % transformed, gender neutral")]
        public string Form_TFMessage_Completed_1st { get; set; }

        // 1st person male specific

         [Display(Name = "1st person 0-20 % transformed, male specific")]
        public string Form_TFMessage_20_Percent_1st_M { get; set; }

        [Display(Name = "1st person 20-40 % transformed, male specific")]
        public string Form_TFMessage_40_Percent_1st_M { get; set; }

        [Display(Name = "1st person 40-60 % transformed, male specific")]
        public string Form_TFMessage_60_Percent_1st_M { get; set; }

        [Display(Name = "1st person 60-80 % transformed, male specific")]
        public string Form_TFMessage_80_Percent_1st_M { get; set; }

        [Display(Name = "1st person 80-99 % transformed, male specific")]
        public string Form_TFMessage_100_Percent_1st_M { get; set; }

        [Display(Name = "1st person 100 % transformed, male specific")]
        public string Form_TFMessage_Completed_1st_M { get; set; }


        //  person female specific

        [Display(Name = "1st person 0-20 % transformed, female specific")]
        public string Form_TFMessage_20_Percent_1st_F { get; set; }

        [Display(Name = "1st person 20-40 % transformed, female specific")]
        public string Form_TFMessage_40_Percent_1st_F { get; set; }

        [Display(Name = "1st person 40-60 % transformed, female specific")]
        public string Form_TFMessage_60_Percent_1st_F { get; set; }

        [Display(Name = "1st person 60-80 % transformed, female specific")]
        public string Form_TFMessage_80_Percent_1st_F { get; set; }

        [Display(Name = "1st person 80-99 % transformed, female specific")]
        public string Form_TFMessage_100_Percent_1st_F { get; set; }

        [Display(Name = "1st person 100 % transformed, female specific")]
        public string Form_TFMessage_Completed_1st_F { get; set; }

        // 3rd person gender neutral

        [Display(Name = "3rd person 0-20 % transformed, gender neutral")]
        public string Form_TFMessage_20_Percent_3rd { get; set; }

        [Display(Name = "3rd person 20-40 % transformed, gender neutral")]
        public string Form_TFMessage_40_Percent_3rd { get; set; }

        [Display(Name = "3rd person 40-60 % transformed, gender neutral")]
        public string Form_TFMessage_60_Percent_3rd { get; set; }

        [Display(Name = "3rd person 60-80 % transformed, gender neutral")]
        public string Form_TFMessage_80_Percent_3rd { get; set; }

         [Display(Name = "3rd person 80-99 % transformed, gender neutral")]
        public string Form_TFMessage_100_Percent_3rd { get; set; }

         [Display(Name = "3rd person 100 % transformed, gender neutral")]
        public string Form_TFMessage_Completed_3rd { get; set; }

        // 3rd person male specific

        [Display(Name = "3rd person 0-20 % transformed, male specific")]
        public string Form_TFMessage_20_Percent_3rd_M { get; set; }

        [Display(Name = "3rd person 20-40 % transformed, male specific")]
        public string Form_TFMessage_40_Percent_3rd_M { get; set; }

        [Display(Name = "3rd person 40-60 % transformed, male specific")]
        public string Form_TFMessage_60_Percent_3rd_M { get; set; }

        [Display(Name = "3rd person 60-80 % transformed, male specific")]
        public string Form_TFMessage_80_Percent_3rd_M { get; set; }

        [Display(Name = "3rd person 80-99 % transformed, male specific")]
        public string Form_TFMessage_100_Percent_3rd_M { get; set; }

        [Display(Name = "3rd person 100 % transformed, male specific")]
        public string Form_TFMessage_Completed_3rd_M { get; set; }

        // 3rd person female specific

        [Display(Name = "3rd person 0-20 % transformed, female specific")]
        public string Form_TFMessage_20_Percent_3rd_F { get; set; }

        [Display(Name = "3rd person 20-40 % transformed, female specific")]
        public string Form_TFMessage_40_Percent_3rd_F { get; set; }

        [Display(Name = "3rd person 40-60 % transformed, female specific")]
        public string Form_TFMessage_60_Percent_3rd_F { get; set; }

        [Display(Name = "3rd person 60-80 % transformed, female specific")]
        public string Form_TFMessage_80_Percent_3rd_F { get; set; }

        [Display(Name = "3rd person 80-99 % transformed, female specific")]
        public string Form_TFMessage_100_Percent_3rd_F { get; set; }

        [Display(Name = "3rd person 100 % transformed, female specific")]
        public string Form_TFMessage_Completed_3rd_F { get; set; }

#endregion form

        // item fields
        #region item

         [Display(Name = "Name of this item.  This MUST match the form name.")]
        public string Item_FriendlyName { get; set; }

         [Display(Name = "Description of this item.  Write this in the third person perspective, ie:  'This item gives its owner...'")]
         public string Item_Description { get; set; }


      //  public decimal MoneyValue { get; set; }

         [Display(Name = "What type of item is this?  (Available choices:  shoes, underpants, pants, undershirt, shirt, hat, accessory, and consumable-reusable)")]
         public string Item_ItemType { get; set; }

         [Display(Name = "How many turns must pass before this item can be reused?  (Only if item type is consumable-reuseable)")]
         public int Item_UseCooldown { get; set; }

        [Display(Name = "What bonuses or penalties does wearing/equipping give to the owner?")]
         public string Item_Bonuses { get; set; }

        [Display(Name = "(Optional) Target animate form of this item/pet's transformation curse")]
        public int? CursedTF_FormSourceId { get; set; }

        [Display(Name = "(Optional) Text shown to the OWNER of this item/pet when it tries to transform its owner but FAILS.  Gender-neutral.")]
        public string CursedTF_Fail { get; set; }

        [Display(Name = "(Optional) Text shown to the OWNER of this item/pet when it tries to transform its owner but FAILS.  Male-specific.")]
        public string CursedTF_Fail_M { get; set; }

        [Display(Name = "(Optional) Text shown to the OWNER of this item/pet when it tries to transform its owner but FAILS.  Female-specific.")]
        public string CursedTF_Fail_F { get; set; }

        [Display(Name = "(Optional) Text shown to the OWNER of this item/pet when it tries to transform its owner and SUCCEEDS.  Gender-neutral.")]
        public string CursedTF_Succeed { get; set; }

        [Display(Name = "(Optional) Text shown to the OWNER of this item/pet when it tries to transform its owner and SUCCEEDS.  Male-specific.")]
        public string CursedTF_Succeed_M { get; set; }

        [Display(Name = "(Optional) Text shown to the OWNER of this item/pet when it tries to transform its owner and SUCCEEDS.  Female-specific.")]
        public string CursedTF_Succeed_F { get; set; }

        [Display(Name = "Text shown to the animate player using this consumable type item.")]
        public string Item_UsageMessage_Player { get; set; }

        [Display(Name = "Text shown to the item that is being used.")]
        public string Item_UsageMessage_Item { get; set; }

        #endregion

        [Display(Name = "Are you ready for an administrator to review this form and include it in the game?  Check below if yes, otherwise leave unchecked.")]
        public bool IsReadyForReview { get; set; }

        public bool IsLive { get; set; }

        public bool AdminApproved { get; set; }

        public DateTime CreationTimestamp { get; set; }

        [Display(Name = "Is there an artist with whom you have arranged to do this artwork?  If so, enter their name here, as well as any special notes to them.  If the artwork is completed, LEAVE A LINK TO THE IMAGE HERE.  If no artwork has been created for this and no artist arranged to provide it, leave this box empty.")]
        public string AssignedToArtist { get; set; }

        public bool ProofreadingCopy { get; set; }

        public bool ProofreadingLockIsOn { get; set; }

        public string CheckedOutBy { get; set; }

         [Display(Name = "Has a proofreader made changes to this spell and it needs to be republished?  Check here if so.")]
        public bool NeedsToBeUpdated { get; set; }

        [Display(Name = "If there is anything that does not belong in a field--questions, explanations, notices, etc--please write them in here.  Proofreaders and administrators can also use this to respond to the original author.")]
        public string Notes { get; set; }

        public int ProofreadingCopyForOriginalId { get; set; }

        public string History { get; set; }

        public string ImageURL { get; set; }

        public bool IsNonstandard { get; set; }

        public decimal HealthBonusPercent { get; set; }
        public decimal ManaBonusPercent { get; set; }
        public decimal ExtraSkillCriticalPercent { get; set; }
        public decimal HealthRecoveryPerUpdate { get; set; }
        public decimal ManaRecoveryPerUpdate { get; set; }
        public decimal SneakPercent { get; set; }
        public decimal EvasionPercent { get; set; }
        public decimal EvasionNegationPercent { get; set; }
        public decimal MeditationExtraMana { get; set; }
        public decimal CleanseExtraHealth { get; set; }
        public decimal MoveActionPointDiscount { get; set; }
        public decimal SpellExtraTFEnergyPercent { get; set; }
        public decimal SpellExtraHealthDamagePercent { get; set; }
        public decimal CleanseExtraTFEnergyRemovalPercent { get; set; }
        public decimal SpellMisfireChanceReduction { get; set; }
        public decimal SpellHealthDamageResistance { get; set; }
        public decimal SpellTFEnergyDamageResistance { get; set; }
        public decimal ExtraInventorySpace { get; set; }

        public float Discipline { get; set; }
        public float Perception { get; set; }
        public float Charisma { get; set; }
        public float Submission_Dominance { get; set; }

        public float Fortitude { get; set; }
        public float Agility { get; set; }
        public float Allure { get; set; }
        public float Corruption_Purity { get; set; }

        public float Magicka { get; set; }
        public float Succour { get; set; }
        public float Luck { get; set; }
        public float Chaos_Order { get; set; }

        public int? SkillSourceId { get; set; }
        public int? FormSourceId { get; set; }
        public int? ItemSourceId { get; set; }
        public string AllowedEditor { get; set; }

        /// <summary>
        /// Returns the name of the folder to look inside to find the appropriate graphic
        /// </summary>
        /// <returns></returns>
        public string GetImageFolderName()
        {
            if (this.Form_MobilityType == PvPStatics.MobilityFull)
            {
                return "portraits";
            }
            else if (this.Form_MobilityType == PvPStatics.MobilityInanimate)
            {
                return "itemsPortraits";
            }
            else if (this.Form_MobilityType == PvPStatics.MobilityPet)
            {
                return "animalPortraits";
            }
            return "";
        }

        /// <summary>
        /// Returns true if the server can locate the main, full sized graphic set for this contribution
        /// </summary>
        /// <returns></returns>
        public bool MainImageExists()
        {
            return File.Exists(AppDomain.CurrentDomain.BaseDirectory + PvPStatics.ImageFolder + this.GetImageFolderName() + "/" + this.ImageURL);
        }

        /// <summary>
        /// Returns true if the server can locate the main, full sized graphic set for this contribution
        /// </summary>
        /// <returns></returns>
        public bool ThumbnailImageExists()
        {
            return File.Exists(AppDomain.CurrentDomain.BaseDirectory + PvPStatics.ImageFolder + this.GetImageFolderName() + "/Thumbnails/100/" + this.ImageURL);
        }

    }
}