/* A program to convert distances from metres or feet to one of the following units:
 *     centimetres,
 *     inches,
 *     kilometres,
 *     miles.
 *  
 * Subject: ITD121 Programming Principles
 * Assessment: 1
 * Author: Lam Kwok Shing (Toni)
 * Student No.: N9516778
 * Date: 2015-10-31
 */

using System;

namespace DistanceConverter {
    class Converter {
        /* Constant declaration */
        private const int OPTION_START_INDEX = 0;
        private const int TYPE_START_INDEX = 0;
        private const string QUIT_COMMAND = "\u0011";
        private const int QUIT_COMMAND_RETURN_VALUE = 0;
        private const int EXCEPTION_RETURN_VALUE = -1;
        private const ConsoleColor colorAlertText = ConsoleColor.White;
        private const ConsoleColor colorAlertBackground = ConsoleColor.Red;

        /* Private variable declaration */
        private static string
            messageChooseTaskException,
            messageChooseTypeException,
            messageConvertOverflowException,
            messageDistanceInputValueException,
            messageEnding,
            messageHeadline,
            messagePromptChooseTask,
            messagePromptChooseType,
            messagePromptEnterDistance,
            messagePromptPressAnyKeyToContinue;
        private static string[]
            optionList,
            originalTypes,
            originalTypesSingular,
            resultTypes,
            resultTypesSingular,
            typeList;
        private static int
            typeOfDistanceOriginal,
            typeOfDistanceResult;
        private static double
            valueOfDistanceOriginal,
            valueOfDistanceResult;
        private static bool programRestart;

        /* Main method - Program start here */
        public static void Main(string[] args) {
            ClassInitialization();
            do {
                // Initialize workflow control variable.
                programRestart = false;

                /* Print instruction and main menu, then ask user to choose an option from the menu. */
                PrintHeadingInstruction();
                PrintMainMenu();
                typeOfDistanceResult = PromptToChooseTask();

                /* Check if the user choice is valid to  take further action.
                 * If it's valid, then ask for the value of original distance and print out the conversion result.
                 * At any point of input, if the user choose to go back to menu, then skip the rest of codes of the
                 * current loop. 
                 */
                if (!programRestart
                    && typeOfDistanceResult > OPTION_START_INDEX
                    && typeOfDistanceResult < optionList.Length) {
                    valueOfDistanceOriginal = PromptToEnterDistanceValue();
                    if (!programRestart) {
                        PrintDistanceTypeMenu(typeOfDistanceResult);
                        typeOfDistanceOriginal = PromptToChooseDistanceType();
                        if (!programRestart) {
                            valueOfDistanceResult = ConvertDistanceFromValue(typeOfDistanceResult,
                                                                   typeOfDistanceOriginal,
                                                                   valueOfDistanceOriginal);
                            if (!programRestart) {
                                PrintConversionResult(typeOfDistanceOriginal,
                                                      typeOfDistanceResult,
                                                      valueOfDistanceOriginal,
                                                      valueOfDistanceResult);
                            }
                            PromptToPressAnyKeyToContinue();
                        }
                    }
                }
                /* Loop back if the option is not the last option, which is quit the program. */
            } while (typeOfDistanceResult != optionList.Length);
            PrintEndingMessage();
        }

        /* This method can be configurated from case to case.
         * Any derived classes can change this method depends on their requirement.
         */
        protected static void ClassInitialization() {
            int optionIndex;
            int maxOption = 5;
            int maxType = 2;

            originalTypes = new string[] { "metres", "feet" };
            originalTypesSingular = new string[] { "metre", "foot" };
            resultTypes = new string[] { "centimetres", "inches", "kilometres", "miles" };
            resultTypesSingular = new string[] { "centimetre", "inch", "kilometre", "mile" };

            /* Define texts for the main menu. */
            optionList = new String[maxOption];
            for (optionIndex = OPTION_START_INDEX; optionIndex < maxOption - 1; optionIndex++) {
                optionList[optionIndex] = "[" + (optionIndex + 1) + "] Convert from metres or feet to "
                                          + resultTypes[optionIndex];
            }
            /* The last option of the main menu should always be the quit option */
            optionList[optionIndex] = "[" + (optionIndex + 1) + "] Quit the program";
            messagePromptChooseTask = "Please enter option [" + (OPTION_START_INDEX + 1) + "-" + (maxOption) + "]: ";
            messageChooseTaskException = "Invalid option. Please try again.";

            /* Define texts for prompting the distance value. */
            messagePromptEnterDistance = "Please enter a distance value: ";
            messageDistanceInputValueException = "Invalid distance value.\n"
                                                 + "The value is either not a decimal number or out of range.";

            /* Define texts for prompting the original distance unit type. */
            typeList = new String[maxType];
            for (optionIndex = TYPE_START_INDEX; optionIndex < maxType; optionIndex++) {
                typeList[optionIndex] = "[" + (optionIndex + 1) + "] Convert from " + originalTypes[optionIndex];
            }
            messagePromptChooseType = "Please enter option [" + (TYPE_START_INDEX + 1) + "-" + (maxType) + "]: ";
            messageChooseTypeException = "Invalid option. Please try again.";

            /* Define other texts */
            messageHeadline = "Enter [Ctrl+Q] at any time to go back to the main menu.";
            messagePromptPressAnyKeyToContinue = "Please press any key to continue...";
            messageEnding = "Program ends.";
            messageConvertOverflowException = "The result is too larger. Program will be restarted.";
        }

        /* This method clear the console window and print the headling message when the program start/restart. */
        private static void PrintHeadingInstruction() {
            Console.Clear();
            Console.WriteLine(messageHeadline);
        }

        /* This method print the value from the optionList array line by line. */
        private static void PrintMainMenu() {
            Console.WriteLine();
            for (int optionIndex = OPTION_START_INDEX; optionIndex < optionList.Length; optionIndex++) {
                Console.WriteLine(optionList[optionIndex]);
            }
        }

        /* This method prompt the user to choose a option from the menu and check if the input is valid. */
        private static int PromptToChooseTask() {
            string userInput;
            int parsedOption;
            bool retry;

            Console.WriteLine();

            /* Loop to prompt user input until the input is valid */
            do {
                retry = false;
                Console.Write(messagePromptChooseTask);
                userInput = Console.ReadLine();
                if (userInput == QUIT_COMMAND) {
                    programRestart = true;
                    return QUIT_COMMAND_RETURN_VALUE;
                } else if (int.TryParse(userInput, out parsedOption)
                            && parsedOption > OPTION_START_INDEX
                            && parsedOption <= optionList.Length) {
                    return parsedOption;
                } else {
                    PrintErrorMessage(messageChooseTaskException);
                    retry = true;
                }
            } while (retry);

            return EXCEPTION_RETURN_VALUE;
        }

        /* This method ask user to enter the distance value to be converted. */
        private static double PromptToEnterDistanceValue() {
            string userInput;
            double parsedValue;
            bool retry = false;

            Console.WriteLine();

            /* Loop to prompt user input until the input is valid */
            do {
                Console.Write(messagePromptEnterDistance);
                userInput = Console.ReadLine();
                if (userInput == QUIT_COMMAND) {
                    programRestart = true;
                    return QUIT_COMMAND_RETURN_VALUE;
                } else if (double.TryParse(userInput, out parsedValue)) {
                    return parsedValue;
                } else {
                    PrintErrorMessage(messageDistanceInputValueException);
                    retry = true;
                }
            } while (retry);

            return EXCEPTION_RETURN_VALUE;
        }

        /* This method print the original distance unit type menu for user to choose. */
        private static void PrintDistanceTypeMenu(int typeOfDistanceResult) {
            Console.WriteLine();
            for (int typeIndex = TYPE_START_INDEX; typeIndex < typeList.Length; typeIndex++) {
                Console.WriteLine("{0} to {1}", typeList[typeIndex], resultTypes[typeOfDistanceResult - 1]);
            }
        }

        /* This method prompt the user to choose a type from the menu and check if the input is valid. */
        private static int PromptToChooseDistanceType() {
            string userInput;
            int parsedOption;
            bool retry = false;

            Console.WriteLine();

            /* Loop to prompt user input until the input is valid */
            do {
                Console.Write(messagePromptChooseType);
                userInput = Console.ReadLine();
                if (userInput == QUIT_COMMAND) {
                    programRestart = true;
                    return QUIT_COMMAND_RETURN_VALUE;
                } else if (int.TryParse(userInput, out parsedOption)
                            && parsedOption > TYPE_START_INDEX
                            && parsedOption <= typeList.Length) {
                    return parsedOption;
                } else {
                    PrintErrorMessage(messageChooseTypeException);
                    retry = true;
                }
            } while (retry);

            return EXCEPTION_RETURN_VALUE;
        }

        /* This method convert the distance from the inputted value based on user choices. */
        private static double ConvertDistanceFromValue(int typeTo, int typeFrom, double originalValue) {
            double[,] coefficient = new double[2, 4]{
                {   // coefficient converted from metres
                    100,                            // 1 metre = 100 centimetres
                    1.0 / 0.0254,                   // 1 metre = 1 / 0.0254 inches
                    1.0 / 1000,                     // 1 metre = 1 / 1000 kilometres
                    1.0 / (1760 * 3 * 12 * 0.0254)  // 1 metre = 1760 * 3 * 12 * 0.0254 miles
                },
                {   // coefficient converted from feet
                    30.48,                          // 1 foot = 30.48 centimetres
                    12,                             // 1 foot = 12 inches
                    30.48 / (100 * 1000),           // 1 foot = 30.48 / (100 * 1000) kilometres
                    1.0 / (1760 * 3)                // 1 foot = 1 / (1760 * 3) miles
                }
            };
            double result;

            /* Check if the coefficient index is in valid range. */
            if ((typeFrom <= 0 || typeFrom > coefficient.GetLength(0))
                && (typeTo <= 0 || typeTo > coefficient.GetLength(1))) {
                return EXCEPTION_RETURN_VALUE;
            } else {
                result = originalValue * coefficient[typeFrom - 1, typeTo - 1];

                /* Check if the converted value is not overflowed the double data type. */
                if (result > Double.MinValue && result < Double.MaxValue) {
                    return result;
                } else {
                    PrintErrorMessage(messageConvertOverflowException);
                    programRestart = true;
                    return EXCEPTION_RETURN_VALUE;
                }
            }
        }

        /* This method print out the result on the console window. */
        private static void PrintConversionResult(int typeFrom, int typeTo, double valueOriginal, double valueResult) {
            string originalUnit, resultUnit;
            /* if the value is equal to 1, then the unit should be in sigular form, otherwise in plural form. */
            if (valueOriginal == 1) {
                originalUnit = originalTypesSingular[typeFrom - 1];
            } else {
                originalUnit = originalTypes[typeFrom - 1];
            }
            if (valueResult == 1) {
                resultUnit = resultTypesSingular[typeTo - 1];
            } else {
                resultUnit = resultTypes[typeTo - 1];
            }

            Console.WriteLine("\n{2} {0} = {3:F3} {1}",
                             originalUnit, resultUnit, valueOriginal, valueResult);
        }

        /* This method prompt the user to press any key to continue and wait for keyboard event. */
        private static void PromptToPressAnyKeyToContinue() {
            Console.Write(messagePromptPressAnyKeyToContinue);
            Console.ReadKey();
        }

        /* This method print out the given error message in speicial colour. */
        private static void PrintErrorMessage(string messageOut) {
            Console.ForegroundColor = colorAlertText;
            Console.BackgroundColor = colorAlertBackground;
            Console.WriteLine(messageOut+"\n");
            Console.ResetColor();
        }

        /* This method print out the program ending message. */
        private static void PrintEndingMessage() {
            Console.WriteLine(messageEnding);
        }
    }
}
