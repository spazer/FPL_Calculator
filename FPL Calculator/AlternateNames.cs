using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace FPL_Calculator
{
    class AlternateNames
    {
        ErrorWriter errorwriter;

        public AlternateNames(ErrorWriter ewriter) 
        {
            errorwriter = ewriter;
        }

        /// <summary>
        /// Returns an alt present in the passed dictionary
        /// </summary>
        /// <param name="nick"></param>
        public string FindPlayerAlt(string nick, Dictionary<string,Player>.KeyCollection allNames)
        {
            StreamReader reader = new StreamReader(".//AlternateNames.txt");
            string input;
            string newNick;

            do
            {
                // Get line of alts
                input = reader.ReadLine();
                input = input.ToUpper();

                // See if the current line of alts contains the input nick
                if (input.Contains(nick.ToUpper()))
                {
                    int index = 0;
                    while (true)
                    {
                        // Parse a nick
                        int commaLocation = input.IndexOf(",", index);
                        if (commaLocation == -1)
                        {
                            // Get all remaining characters for a final match attempt
                            newNick = input.Substring(index, input.Length - index).Trim();
                            if (allNames.Contains(newNick))
                            {
                                errorwriter.Write("Match found: " + newNick);
                                reader.Close();
                                return newNick;
                            }
                            break;
                        }
                        else
                        {
                            newNick = input.Substring(index, commaLocation - index);

                            // If the new nick is found in the collection of players, return the new nick
                            if (allNames.Contains(newNick))
                            {
                                errorwriter.Write("Match found: " + newNick);
                                reader.Close();
                                return newNick;
                            }

                            // Advance past the comma
                            index = commaLocation + 1;
                        }
                    }
                }
            } while (!reader.EndOfStream);

            // Search failed
            errorwriter.Write("No match found");
            reader.Close();
            return string.Empty;
        }
    }
}
