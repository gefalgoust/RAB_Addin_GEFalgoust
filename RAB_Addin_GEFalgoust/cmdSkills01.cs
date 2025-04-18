namespace RAB_Addin_GEFalgoust
{
    [Transaction(TransactionMode.Manual)]
    public class cmdSkills01 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            // 1. set variables
            int numFloors = 250;
            double currentElev = 0;
            int floorHeight = 15;
            int fizzCount = 0;
            int buzzCount = 0;
            int fizzBuzzCount = 0;

            // 9. get titleblock type
            FilteredElementCollector tbCollector = new FilteredElementCollector(doc);
            tbCollector.OfCategory(BuiltInCategory.OST_TitleBlocks);
            tbCollector.WhereElementIsElementType();
            ElementId tblockId = tbCollector.FirstElementId();

            // 10. get view family types
            FilteredElementCollector vftCollector = new FilteredElementCollector(doc);
            vftCollector.OfClass(typeof(ViewFamilyType));

            ViewFamilyType fpVFT = null;
            ViewFamilyType cpVFT = null;

            foreach (ViewFamilyType curVFT in vftCollector)
            {
                if (curVFT.ViewFamily == ViewFamily.FloorPlan)
                {
                    fpVFT = curVFT;
                }
                else if (curVFT.ViewFamily == ViewFamily.CeilingPlan)
                {
                    cpVFT = curVFT;
                }
            }

            // 10. create transaction
            Transaction t = new Transaction(doc);
            t.Start("FIZZ BUZZ Challenge");

            // 2. loop through floors and check FIZZBUZZ
            for (int i = 1; i <= numFloors; i++)
            {
                // 3. create level
                Level newLevel = Level.Create(doc, currentElev);
                newLevel.Name = "LEVEL " + i.ToString();

                // 4. increment elevation
                currentElev += floorHeight;

                // 5. check for FIZZBUZZ
                if (i % 3 == 0 && i % 5 == 0)
                {
                    // 6. FIZZBUZZ - create sheet
                    ViewSheet newSheet = ViewSheet.Create(doc, tblockId);
                    newSheet.SheetNumber = i.ToString();
                    newSheet.Name = "FIZZBUZZ_" + i.ToString();

                    // BONUS
                    ViewPlan bonusPlan = ViewPlan.Create(doc, fpVFT.Id, newLevel.Id);
                    bonusPlan.Name = "FIZZBUZZ_" + i.ToString();

                    Viewport newVP = Viewport.Create(doc, newSheet.Id, bonusPlan.Id, new XYZ(1.25, 1, 0));

                    fizzBuzzCount++;
                }
                else if (i % 3 == 0)
                {
                    // 7. FIZZ - create floor plan
                    ViewPlan newFloorPlan = ViewPlan.Create(doc, fpVFT.Id, newLevel.Id);
                    newFloorPlan.Name = "FIZZ_" + i.ToString();
                    fizzCount++;
                }
                else if (i % 5 == 0)
                {
                    // 8. BUZZ - create ceiling plan
                    ViewPlan newCeilingPlan = ViewPlan.Create(doc, cpVFT.Id, newLevel.Id);
                    newCeilingPlan.Name = "BUZZ_" + i.ToString();
                    buzzCount++;
                }

            }

            // 11. commit transaction
            t.Commit();
            t.Dispose();

            // 6. alert user
            string resultString = $"Created {numFloors} levels. {fizzBuzzCount} FIZZBUZZ. {fizzCount} FIZZ. {buzzCount} BUZZ.";
            TaskDialog.Show("Complete", resultString);

            return Result.Succeeded;
        }
    }

}
