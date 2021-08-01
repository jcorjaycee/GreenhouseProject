// Sean Leckie 0919266
// INFO-3138 Project 2
// Submitted: independently, after enrolment deadline (late enrolment penalty), <24h late (10% late penalty)

using System;
using System.Collections.Generic;
using System.Xml;           // XmlDocument (DOM) class
using System.Xml.XPath;     // XPathNavigator class

namespace GreenhouseProject
{
    class Program
    {
        private const string FILE_PATH = "ghg-canada.xml"; // path to XML, resides in bin file
        private static XmlDocument _doc = null; // allows us to reference the XML doc program-wide

        // referenced program-wide to display data within a range of dates
        static int startYear = 2015;
        static int endYear = 2019;

        static void Main()
        {
            while (true)
            {
                // load the XML doc
                try
                {
                    _doc = new XmlDocument();
                    _doc.Load(FILE_PATH);
                } catch (Exception)
                {
                    Console.WriteLine("Looks like " + FILE_PATH + " is missing. Maybe it's not in the right folder?");
                    Console.WriteLine("We can't operate on a file that doesn't exist. Come back when it's in its place!");
                    return;
                }

                // header
                Console.WriteLine(new string('-', 34));
                Console.WriteLine(" Greenhouse Gas Emissions in Canada ");
                Console.WriteLine(new string('-', 34));
                Console.WriteLine("");
                Console.WriteLine("'Y' to adjust the range of years");
                Console.WriteLine("'R' to select a region");
                Console.WriteLine("'S' to select a specific GHG source");
                Console.WriteLine("'X' to exit the program");
                Console.Write("Your selection: ");

                char selection = Char.ToUpper((char)Console.ReadKey().Key);

                Console.WriteLine("\n");

                switch (selection)
                {
                    case 'Y':
                        ModifyYearRange();
                        break;
                    case 'R':
                        SelectRegion();
                        break;
                    case 'S':
                        SelectSource();
                        break;
                    case 'X':
                        return;
                    default:
                        Console.WriteLine("Invald entry. Press any key to continue.");
                        Console.ReadKey();
                        break;
                }

                Console.Clear();
            }
        }

        // ModifyYearRange function
        // purpose: serves as a separate menu to allow user to enter a date range
        static void ModifyYearRange()
        {
            // these variables allow us to avoid modifying the old data, should we be interrputed
            int newStartYear = startYear;
            int newEndYear = endYear;

            // controls loops for valid user input
            bool complete = false;

            Console.WriteLine("Starting year (1990 through 2019, or blank to leave as is):");
            while (!complete) // run until valid input
            {
                string input = Console.ReadLine().ToUpper();
                if (input.Trim() == "X") // if user chooses to return to main menu
                    return;
                if (input.Trim() == "") // if user chooses not to change this var
                    break;
                try
                {
                    if (int.Parse(input) < 1990 || int.Parse(input) > 2019)
                        throw new FormatException("Date was not between 1990 and 2019");
                    complete = true;
                    newStartYear = int.Parse(input);
                }
                catch (FormatException)
                {
                    Console.WriteLine("This is not a valid input. Please input a date between 1990 and 2019, or input X to cancel.");
                }
            }

            complete = false;
            Console.WriteLine($"Ending year ({newStartYear} through {newStartYear + 4}, or blank to leave as is):");

            while (!complete) // run until valid input
            {
                string input = Console.ReadLine().ToUpper();
                Console.WriteLine();

                try
                {
                    if (input.Trim() == "X") // if user chooses to return to main menu
                        return;
                    if (input.Trim() == "") // if user chooses not to change this var
                        if (newEndYear < newStartYear || endYear > newStartYear + 4)
                            throw new Exception($"Cannot keep same end year, must between {newStartYear} and {newStartYear + 4} (current is {endYear})");
                        else
                            break;
                    if (int.Parse(input) < newStartYear || int.Parse(input) > newStartYear + 5)
                        throw new Exception($"Date was not between {newStartYear} and {newStartYear + 4}");
                    complete = true;
                    newEndYear = int.Parse(input);
                }
                catch (FormatException)
                {
                    Console.WriteLine($"This is not a valid input. Please input a date between {newStartYear} and {newStartYear + 4}, or input X to cancel.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            // all is well, commit the new dates!
            startYear = newStartYear;
            endYear = newEndYear;
        }

        // SelectRegion function
        // purpose: serves as a separate menu to allow user to select a region to view emmissions data from
        static void SelectRegion()
        {
            Console.WriteLine("Enter a region by number as shown below, then press return...");

            // create NodeIterator
            XPathNavigator nav = _doc.CreateNavigator();
            XPathNodeIterator nodeIt = nav.Select("//region/@name");

            while (nodeIt.MoveNext())
            {
                Console.Write(" ");
                if (nodeIt.CurrentPosition < 10)
                    Console.Write(" "); // extra formatting space
                Console.WriteLine(nodeIt.CurrentPosition + ". " + nodeIt.Current.Value); // prints out all region names with numbers using XPath
            }

            while (true)
            {
                string input = Console.ReadLine();
                if (input.Trim().ToUpper() == "X") // if user chooses to return to main menu
                    return;

                try
                {
                    int parsedInput = int.Parse(input);
                    if (parsedInput < 1 || parsedInput > nodeIt.Count)
                    {
                        throw new Exception("The input is outside the range of regions. Please input the number of the region, or X to cancel.");
                    }

                    XPathNodeIterator regionNodeIt = nav.Select($"//region[position() = {parsedInput}]/@name"); // returns just one value from position()
                    regionNodeIt.MoveNext(); // grab value
                    Console.WriteLine();
                    PrintReport("region", regionNodeIt.Current.Value);
                }
                catch (FormatException)
                {
                    Console.WriteLine("This is an invalid input. Please input the number of the region, or X to cancel.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
        }

        // SelectSource function
        // purpose: serves as a separate menu to allow user to select an emmissions type to view
        static void SelectSource()
        {
            Console.WriteLine("Enter a source by number as shown below, then press enter......");

            XPathNavigator nav = _doc.CreateNavigator();
            XPathNodeIterator nodeIt = nav.Select("//source[not(@description=preceding::source/@description)]/@description");

            while (nodeIt.MoveNext())
            {
                Console.Write(" ");
                if (nodeIt.CurrentPosition < 10)
                    Console.Write(" "); // extra formatting space
                Console.WriteLine(nodeIt.CurrentPosition + ". " + nodeIt.Current.Value); // prints out all source names with numbers using XPath
            }

            while (true)
            {
                string input = Console.ReadLine();
                if (input.Trim().ToUpper() == "X") // if user chooses to return to main menu
                    return;

                try
                {
                    int parsedInput = int.Parse(input);
                    if (parsedInput < 1 || parsedInput > nodeIt.Count)
                    {
                        throw new Exception("The input is outside the range of regions. Please input the number of the source, or X to cancel.");
                    }

                    XPathNodeIterator regionNodeIt = nav.Select($"//source[position() = {parsedInput}]/@description"); // returns just one value from position()
                    regionNodeIt.MoveNext(); // grab value
                    Console.WriteLine();
                    PrintReport("source", regionNodeIt.Current.Value);
                }
                catch (FormatException)
                {
                    Console.WriteLine("This is an invalid input. Please input the number of the source, or X to cancel.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
        }

        // PrintReport function
        // purpose: prints a report to the console for the requested data
        // accepts: string type (for conditional), string subject (for XPath lookups)
        // note: I decided to do this in one function since there is a significant similarity between printing
        // a region report and a source report such that it is efficient to do in a conditional
        static void PrintReport(string type, string subject)
        {

            // create shared NodeIterators
            XPathNavigator nav = _doc.CreateNavigator();
            XPathNodeIterator nodeIt;
            XPathNodeIterator attributeNodeIt;

            // used to build strings for output to console
            string outputString;

            //
            List<string> attributeList = new List<string>();

            switch (type)
            {
                case "region":

                    // get all possible sources from XPath
                    // This XPath uses a couple special operators, such as `not` and `preceding::` to not duplicate the sources in the list
                    attributeNodeIt = nav.Select("//source[not(@description=preceding::source/@description)]/@description"); 
                    while (attributeNodeIt.MoveNext())
                    {
                        attributeList.Add(attributeNodeIt.Current.Value);
                    }

                    // this string is a bit redundant, is only used for outputString.Length
                    outputString = "Emissions in " + subject + " (Megatonnes)";

                    Console.WriteLine(outputString);
                    Console.WriteLine(new string('-', outputString.Length));
                    Console.WriteLine();

                    // begin building table header string
                    outputString = "Source";

                    for (int i = 0; i < (endYear - startYear + 1); i++) // for each year
                    {
                        outputString += (startYear + i).ToString().PadLeft(9, ' '); // add year to output string
                    }

                    Console.WriteLine(outputString.PadLeft(100, ' '));
                    Console.WriteLine();

                    for (int i = 0; i < attributeList.Count; i++) // for each source of emissions
                    {
                        nodeIt = 
                            nav.Select($"//region[@name = '{subject}']/source[@description = '{attributeList[i]}']/emissions[@year >= {startYear} and @year <= {endYear}]");
                        outputString = attributeList[i]; // add the source to the first column
                        while (nodeIt.MoveNext()) // add the values to each subsequent column
                        {
                            double numberFormat = Math.Round(double.Parse(nodeIt.Current.Value), 3); // parse the value from the iterator, round it,
                            outputString += numberFormat.ToString("0.###").PadLeft(9, ' '); // format the string, pad it, and add it to the outputString
                        }

                        Console.WriteLine(outputString.PadLeft(100, ' '));
                    }
                    break;

                case "source":

                    // get all possible regions from XPath
                    attributeNodeIt = nav.Select("//region/@name");
                    while (attributeNodeIt.MoveNext())
                    {
                        attributeList.Add(attributeNodeIt.Current.Value);
                    }

                    // this string is a bit redundant, is only used for outputString.Length
                    outputString = "Emissions from " + subject + " (Megatonnes)";

                    Console.WriteLine(outputString);
                    Console.WriteLine(new string('-', outputString.Length));
                    Console.WriteLine();

                    // begin building table header string
                    outputString = "Region";

                    for (int i = 0; i < (endYear - startYear + 1); i++) // for each year
                    {
                        outputString += (startYear + i).ToString().PadLeft(9, ' '); // add year to output string
                    }

                    Console.WriteLine(outputString.PadLeft(100, ' '));
                    Console.WriteLine();

                    int pastYear = startYear - 1;
                    int currentYear = startYear;
                    int lineCounter = 0;

                    for (int i = 0; i < attributeList.Count; i++) // for each region
                    {
                        nodeIt = 
                            nav.Select($"//region[@name = '{attributeList[i]}']/source[@description = '{subject}']/emissions[@year >= {startYear} and @year <= {endYear}]");
                        outputString = attributeList[i]; // add the source to the first column

                        while (nodeIt.MoveNext()) // add the values to each subsequent column
                        {
                            currentYear = int.Parse(nodeIt.Current.GetAttribute("year", "")); // we need to get the year to make sure we haven't skipped!
                            while (currentYear != pastYear + 1) // if data is missing
                            {
                                outputString += new string("-").PadLeft(9, ' ');
                                pastYear++;
                                lineCounter++;
                            }
                            double numberFormat = Math.Round(double.Parse(nodeIt.Current.Value), 3); // parse the value from the iterator, round it,
                            outputString += numberFormat.ToString("0.###").PadLeft(9, ' '); // format the string, pad it, and add it to the outputString
                            // set the counter variables to new values
                            pastYear = currentYear;
                            lineCounter++;
                        }
                        
                        // there should always be n pieces of data output, where n = endYear - startYear + 1
                        // ex dates 1990 to 1994, that's 5 years. 1994 - 1990 = 4, 4 + 1 = 5
                        // this conditional essentially catches the case where the last piece of missing data is skipped over
                        if (nodeIt.CurrentPosition <= (endYear - startYear))
                        {
                            while (lineCounter != (endYear - startYear + 1))
                            {
                                outputString += new string("-").PadLeft(9, ' ');
                                lineCounter++;
                            }
                        }

                        // reset counter vars for the next iteration of the loop
                        pastYear = startYear - 1;
                        lineCounter = 0;

                        Console.WriteLine(outputString.PadLeft(100, ' '));
                    }

                    break;
            }
        }
    }
}
