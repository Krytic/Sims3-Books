/**
 * Greetings! If you're reading this, you deserve to know a little background. I've tried to document the code as best as possible.
 * 
 * This was a year-long project because I had to rewrite it several times due to limitations.
 * Originally it was a winform because I thought it was a simple project. ::)
 * Then I rewrote it because winforms were becoming a pain, as a WPF application.
 * Finally I rewrote it into this because I realised the way I was doing it was so wrong, so I made it a game mod.
 * 
 * This basically loads the books XML then iterates through it and translates every entry into a readable format.
 * Eventually I'll set it to be able to generate tables with good info in it. ;)
 * 
 * Finally, I would like to note, this project was started when I was much younger and inexperienced with programming.
 * Other projects from around that time (which I will eventually pick up again) were **hideous**. Whilst I'm putting this
 * on GitHub as a part of my programming journey, it is in no way supposed to be "good".
 * 
 * Enjoy. :)
 * 
 * Usage
 * =====
 * You will need to compile this into a package using s3pe. The namespace for the FNV64 hash is SeabodySoftware.Bookbuilder
 * https://modthesims.info/wiki.php?title=Tutorial:Sims_3_Pure_Scripting_Modding#Building_The_Package
 * 
 * Once done, install as a game mod for The Sims 3. It adds an interaction to your Sims, entitled "Build Book List". Using this
 * dumps every game book as a CSV list into an exception file in Documents/Electronic Arts/The Sims 3.
 *
 * This probably requires every expansion pack. (at least Supernatural and University Life)
 * 
 * License
 * =======
 * This project is licensed under the MIT license.
 * 
 * Changelog 
 * =========
 * Finished into a quasi-usable state on 20/07/2014
 * Finished into a fully usable (albeit requiring workarounds for the final product) on 9/11/14
 * Rebuilt into generating a csv-ish file on 13/08/2024 (ten years later!)
 * 
 * Thanks
 * Seabody / Krytic / ordinarystarman / whatever you know me by now :)
 */

using System;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI;

using Sims3.Gameplay.Objects;

namespace SeabodySoftware
{
    public class BookBuilder
    {
        [Tunable]
        protected static bool kInstantiator = false;

        protected static string SEPCHAR = "|";

        static BookBuilder()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinished);
        }

        private static void OnWorldLoadFinished(object sender, EventArgs e)
        {
            // Adds the Build Book List interaction to every sim in the world.
            foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
            {
                if (sim == null)
                {
                    continue;
                }

                AddInteractions(sim);
            }

            EventTracker.AddListener(EventTypeId.kSimInstantiated, new ProcessEventDelegate(BookBuilder.OnSimInstantiated));
        }

        private static ListenerAction OnSimInstantiated(Event e)
        {
            try
            {
                Sim sim = e.TargetObject as Sim;
                if (sim != null)
                {
                    BookBuilder.AddInteractions(sim);
                }
            }
            catch (Exception)
            {
            }
            return ListenerAction.Keep;
        }

        public static void AddInteractions(Sim sim)
        {
            sim.AddInteraction(ShowNotification.Singleton);
        }

        private sealed class ShowNotification : ImmediateInteraction<Sim, Sim>
        {
            public static readonly InteractionDefinition Singleton = new Definition();

            // Welcome to my personal hell.
            protected override bool Run()
            {

                string CSVListString = string.Format("BookType{0}Title{0}Author{0}Length{0}Value\n", SEPCHAR);
                foreach (BookGeneralData book in BookData.BookGeneralDataList.Values) // General Books
                {
                    string TypeFlag = "General";
                    string ConjoinedBookInformationString = TypeFlag + SEPCHAR + book.Title + SEPCHAR + book.Author + SEPCHAR + book.Length + SEPCHAR + book.Value;
                    CSVListString = CSVListString + ConjoinedBookInformationString + "\n";
                }
                foreach (AcademicTextBookData book in BookData.AcademicTextBookDataList.Values)
                {
                    string TypeFlag = "AcademicTextbook"; // Textbooks (University Life)
                    string ConjoinedBookInformationString = TypeFlag + SEPCHAR + book.Title + SEPCHAR + book.Author + SEPCHAR + book.Length + SEPCHAR + book.Value;
                    CSVListString = CSVListString + ConjoinedBookInformationString + "\n";
                }
                foreach (BookSkillData book in BookData.BookSkillDataList.Values)
                {
                    string TypeFlag = "Skill"; // Skill books
                    string ConjoinedBookInformationString = TypeFlag + SEPCHAR + book.Title + SEPCHAR + book.Author + SEPCHAR + book.Length + SEPCHAR + book.Value;
                    CSVListString = CSVListString + ConjoinedBookInformationString + "\n";
                }
                foreach (BookRecipeData book in BookData.BookRecipeDataList.Values)
                {
                    string TypeFlag = "Recipe"; // Recipe books
                    string ConjoinedBookInformationString = TypeFlag + SEPCHAR + book.Title + SEPCHAR + book.Author + SEPCHAR + book.Length + SEPCHAR + book.Value;
                    CSVListString = CSVListString + ConjoinedBookInformationString + "\n";
                }
                foreach (SheetMusicData book in BookData.SheetMusicDataList.Values)
                {
                    string TypeFlag = "SheetMusic"; // Sheet Music
                    string ConjoinedBookInformationString = TypeFlag + SEPCHAR + book.Title + SEPCHAR + book.Author + SEPCHAR + book.Length + SEPCHAR + book.Value;
                    CSVListString = CSVListString + ConjoinedBookInformationString + "\n";
                }
                foreach (BookToddlerData book in BookData.BookToddlerDataList.Values)
                {
                    string TypeFlag = "Toddler"; // Baby books
                    string ConjoinedBookInformationString = TypeFlag + SEPCHAR + book.Title + SEPCHAR + book.Author + SEPCHAR + book.Length + SEPCHAR + book.Value;
                    CSVListString = CSVListString + ConjoinedBookInformationString + "\n";
                }
                foreach (BookFishData book in BookData.FishBookDataList.Values)
                {
                    string TypeFlag = "Fish"; // Bait books
                    string ConjoinedBookInformationString = TypeFlag + SEPCHAR + book.Title + SEPCHAR + book.Author + SEPCHAR + book.Length + SEPCHAR + book.Value;
                    CSVListString = CSVListString + ConjoinedBookInformationString + "\n";
                }
                foreach (BookAlchemyRecipeData book in BookData.BookAlchemyRecipeDataList.Values)
                {
                    string TypeFlag = "AlchemyRecipe"; // Alchemy Books
                    string ConjoinedBookInformationString = TypeFlag + SEPCHAR + book.Title + SEPCHAR + book.Author + SEPCHAR + book.Length + SEPCHAR + book.Value;
                    CSVListString = CSVListString + ConjoinedBookInformationString + "\n";
                }
                foreach (BookComicData book in BookData.BookComicDataList.Values)
                {
                    string TypeFlag = "Comic"; // Comics
                    string ConjoinedBookInformationString = TypeFlag + SEPCHAR + book.Title + SEPCHAR + book.Author + SEPCHAR + book.Length + SEPCHAR + book.Value;
                    CSVListString = CSVListString + ConjoinedBookInformationString + "\n";
                }

                // Dump this to an xcpt file.
                Exception excp = new Exception(CSVListString);
                new ScriptError(null, excp, 0).WriteMiniScriptError();

                // Notify player that the processing is done.
                base.Actor.ShowTNSIfSelectable("Book list generated.", StyledNotification.NotificationStyle.kSimTalking);

                return true;
            }

            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, Sim, ShowNotification>
            {
                protected override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
                {
                    // Only english (sorry!)
                    return "Build Book List";
                }

                protected override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }
        }
    }
}