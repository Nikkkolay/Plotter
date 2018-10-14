using System;
using System.Collections.Generic;

namespace Buturin.PolishEntry
{
    //возможные типы токена
    public enum TokenType : byte
    {
        Number, Variable, Operator, OpeningParenthesis,
        ClosingParenthesis, Function, EndOfLine, StartOfLine
    };

    ///<summary>
    ///структура хранящая тип токена и его строчное представление
    ///</summary>
    public struct Token { public string Name; public TokenType Type; public sbyte Priority; }

    class ReversePolishEntry
    {
        string[] functions = {  "Abs", "Acos", "Asin", "Atan", "Actg",
                                "Cos", "Cosh", "Sin", "Sinh", "Tan",
                                "Tanh", "Ctg", "Ctgh", "Ln", "Lg",
                                "Sqrt", "Sign"};

        string originalString;
        public string OriginalString
        { get { return originalString; } }

        LinkedList<Token> polishEntry;
        LinkedList<Token> PolishEntry
        { get { return polishEntry; } }

        public ReversePolishEntry()
        {
            originalString = "";
            polishEntry = null;
        }

        /// <summary>
        /// создание обратной польской записи из переданной строки
        /// </summary>
        public ReversePolishEntry(string str)
        {
            originalString = str;
            polishEntry = ToPolishEntry(RemoveUnaryMinusAndPlus(str));
        }

        //пропуск пробелов
        int PassSpace(int currentIndex, string str)
        {
            if (currentIndex > str.Length)
                throw new Exception();
            if (currentIndex == str.Length)
                return currentIndex;

            int i;
            for (i = currentIndex; (i < str.Length) && (str[i] == ' '); i++) ;
            return i;
        }

        //удаление унарных плюсов и замена унарных минусов на (-1)*
        string RemoveUnaryMinusAndPlus(string str)
        {
            int index = 0;

            while (index < str.Length)
            {
                if (str[index] == '+' && (index == 0 || str[index - 1] == '('))
                    str = str.Remove(index, 1);
                else if (str[index] == '-' && (index == 0 || str[index - 1] == '('))
                {
                    int tempindex = PassSpace(index + 1, str);
                    if (!Char.IsNumber(str[tempindex]))
                    {
                        str = str.Remove(index, 1);
                        str = str.Insert(index, "(-1)*");
                        index += 5;
                    }
                    else index = tempindex;

                }
                else
                    index++;
            }
            return str;
        }

        ///получение следующей лексемы
        Token GetToken(ref int currentIndex, string str)
        {
            Token token;

            currentIndex = PassSpace(currentIndex, str);
            if (currentIndex == str.Length)
            {
                token.Type = TokenType.EndOfLine;
                token.Name = "";
                token.Priority = -1;
                return token;
            }

            token.Name = str[currentIndex++].ToString();

            if (token.Name == "+")
            {
                token.Type = TokenType.Operator;
                token.Priority = 0;
                return token;
            }

            if (token.Name == "-")
            {
                int tempIndex = currentIndex;
                tempIndex = PassSpace(tempIndex, str);

                if (Char.IsNumber(str[tempIndex]) && ((currentIndex == 1) || str[currentIndex - 2] == '('))
                {
                    token.Priority = -1;
                    token.Type = TokenType.Number;
                    currentIndex = tempIndex;

                    bool onePoint = false;

                    while (currentIndex < str.Length && (Char.IsNumber(str[currentIndex]) || str[currentIndex] == '.'))
                    {
                        if (str[currentIndex] == '.')
                        {
                            if (!onePoint)
                            {
                                token.Name += str[currentIndex];
                                onePoint = true;
                            }
                            else throw new Exception("Число с 2 символами \".\"");
                        }
                        else token.Name += str[currentIndex];

                        currentIndex++;
                    }


                    return token;
                }
                else
                {
                    token.Type = TokenType.Operator;
                    token.Priority = 0;
                    return token;
                }
            }

            if (token.Name == "*" || token.Name == "/")
            {
                token.Type = TokenType.Operator;
                token.Priority = 1;
                return token;
            }

            if (token.Name == "^")
            {
                token.Type = TokenType.Operator;
                token.Priority = 2;
                return token;
            }

            if (token.Name == "(")
            {
                token.Type = TokenType.OpeningParenthesis;
                token.Priority = -1;
                return token;
            }

            if (token.Name == ")")
            {
                token.Type = TokenType.ClosingParenthesis;
                token.Priority = -1;
                return token;
            }

            if (token.Name == "x" || token.Name == "X")
            {
                token.Type = TokenType.Variable;
                token.Name = "x";
                token.Priority = -1;
                return token;
            }

            if (Char.IsNumber(token.Name[0]))
            {
                token.Priority = -1;

                bool onePoint = false;

                while (currentIndex < str.Length && (Char.IsNumber(str[currentIndex]) || str[currentIndex] == ',' || str[currentIndex] == '.'))
                {
                    if (str[currentIndex] == ',' || str[currentIndex] == '.')
                    {
                        if (!onePoint)
                        {
                            token.Name += ",";
                            onePoint = true;
                        }
                        else throw new Exception("Число с 2 символами \",\"");
                    }
                    else token.Name += str[currentIndex];

                    currentIndex++;
                }

                token.Type = TokenType.Number;
                return token;
            }

            if (Char.IsLetter(token.Name[0]))
            {
                token.Priority = -1;
                token.Type = TokenType.Function;

                while (currentIndex < str.Length && Char.IsLetter(str[currentIndex]))
                {
                    token.Name += str[currentIndex];
                    currentIndex++;
                }

                bool isFunc = false;

                foreach (string Func in functions)
                    if (String.Equals(token.Name, Func, StringComparison.OrdinalIgnoreCase))
                    {
                        token.Name = Func;
                        isFunc = true;
                        break;
                    }

                if (!isFunc)
                    throw new Exception("Unknown function: \"" + token.Name + "\".");

                if (currentIndex == str.Length)
                    throw new Exception("No opening parentheses after function: \"" + token.Name + "\".");
                else if (str[currentIndex] != '(')
                    throw new Exception("No opening parentheses after function: \"" + token.Name + "\".");


                currentIndex++;//пропус '(' стоящей после функции
                return token;
            }


            throw new Exception("An unknown error occurred while trying to read the function.");
        }

        //перобразование строки в строку с польской записью
        LinkedList<Token> ToPolishEntry(string str)
        {

            int index = 0;
            LinkedList<Token> polishEntry_ = new LinkedList<Token>();
            List<Token> tempList = new List<Token>();

            Token previousToken;
            previousToken.Type = TokenType.StartOfLine;
            Token token = GetToken(ref index, str);

            if (token.Type == TokenType.EndOfLine)
                return polishEntry_;

            while (token.Type != TokenType.EndOfLine)
            {
                if (token.Type == TokenType.Number || token.Type == TokenType.Variable)
                {
                    if (previousToken.Type == TokenType.StartOfLine || previousToken.Type == TokenType.OpeningParenthesis ||
                        previousToken.Type == TokenType.Operator || previousToken.Type == TokenType.Function)
                        polishEntry_.AddLast(token);
                    else
                        throw new Exception("Incorrect arrangement of a number or variable in the entered function.");
                }
                else
                {
                    if (token.Type == TokenType.OpeningParenthesis || token.Type == TokenType.Function)
                    {
                        if (previousToken.Type == TokenType.StartOfLine || previousToken.Type == TokenType.OpeningParenthesis ||
                        previousToken.Type == TokenType.Operator || previousToken.Type == TokenType.Function)
                            tempList.Add(token);
                        else
                            throw new Exception("Incorrect arrangement of a opening parenthesis or function in the entered function.");
                    }
                    else if (token.Type == TokenType.ClosingParenthesis)
                    {
                        if (previousToken.Type == TokenType.ClosingParenthesis || previousToken.Type == TokenType.OpeningParenthesis ||
                            previousToken.Type == TokenType.Number || previousToken.Type == TokenType.Variable)
                        {
                            bool findOpeningParenthesis = false;

                            while (tempList.Count != 0)
                            {
                                if (tempList[tempList.Count - 1].Type == TokenType.OpeningParenthesis)
                                {
                                    findOpeningParenthesis = true;
                                    tempList.RemoveAt(tempList.Count - 1);
                                    break;
                                }

                                if (tempList[tempList.Count - 1].Type == TokenType.Function)
                                {
                                    findOpeningParenthesis = true;
                                    polishEntry_.AddLast(tempList[tempList.Count - 1]);
                                    tempList.RemoveAt(tempList.Count - 1);
                                    break;
                                }

                                polishEntry_.AddLast(tempList[tempList.Count - 1]);
                                tempList.RemoveAt(tempList.Count - 1);
                            }

                            if (!findOpeningParenthesis)
                                throw new Exception("the opening parenthesis is missing.");
                        }
                        else
                            throw new Exception("Incorrect arrangement of a closing parenthesis in the entered function.");
                    }
                    else
                    {
                        //здесь обрабатываются операторы
                        if (previousToken.Type == TokenType.Number || previousToken.Type == TokenType.Variable ||
                            previousToken.Type == TokenType.ClosingParenthesis)
                        {
                            if (tempList.Count == 0)
                                tempList.Add(token);
                            else
                            {
                                while (tempList[tempList.Count - 1].Priority > token.Priority
                                    && tempList[tempList.Count - 1].Type != TokenType.OpeningParenthesis
                                    && tempList[tempList.Count - 1].Type != TokenType.Function)
                                {
                                    polishEntry_.AddLast(tempList[tempList.Count - 1]);
                                    tempList.RemoveAt(tempList.Count - 1);

                                    if (tempList.Count == 0)
                                        break;
                                }

                                tempList.Add(token);
                            }
                        }
                        else
                            throw new Exception("Incorrect arrangement of a operator in the entered function.");
                    }

                }

                previousToken = token;
                token = GetToken(ref index, str);
            }

            while (tempList.Count != 0)
            {
                polishEntry_.AddLast(tempList[tempList.Count - 1]);
                tempList.RemoveAt(tempList.Count - 1);
            }

            return polishEntry_;
        }

        /// <summary>
        /// подсчет функции по заданному Х
        /// </summary>
        public double FunctionValue(double x)
        {
            LinkedList<Token> tempList = new LinkedList<Token>();

            foreach (Token token in polishEntry)
            {
                if (token.Type == TokenType.Number)
                { tempList.AddLast(token); continue; }

                if (token.Type == TokenType.Variable)
                { tempList.AddLast(new Token { Name = Convert.ToString(x), Type = TokenType.Number, Priority = -1 }); }

                if (token.Type == TokenType.Operator)
                {
                    if (tempList.Count < 2)
                        throw new Exception("При пытке сложения не хватило чисел.");

                    double b = Convert.ToDouble((tempList.Last).Value.Name);
                    tempList.RemoveLast();
                    double a = Convert.ToDouble((tempList.Last).Value.Name);
                    tempList.RemoveLast();

                    switch (token.Name)
                    {
                        case "+":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(a + b), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "-":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(a - b), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "*":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(a * b), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "/":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(a / b), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "^":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Pow(a, b)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        default:
                            throw new Exception("при попытке подсчета выражения возник неизвестный оператор \"" + token.Name + "\"");

                    }
                }

                if (token.Type == TokenType.Function)
                {
                    if (tempList.Count < 1)
                        throw new Exception("При пытке использовать функцию не хватило чисела.");
                    double a = Convert.ToDouble((tempList.Last).Value.Name);
                    tempList.RemoveLast();

                    switch (token.Name)
                    {
                        case "Abs":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Abs(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Acos":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Acos(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Asin":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Asin(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Atan":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Atan(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Actg":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.PI / 2 - Math.Atan(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Cos":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Cos(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Cosh":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Cosh(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Sin":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Sin(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Sinh":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Sinh(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Tan":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Tan(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Tanh":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Tanh(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Ctg":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(1.0 / Math.Tan(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Ctgh":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(1.0 / Math.Tanh(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Ln":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Log(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Lg":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Log10(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Sqrt":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Sqrt(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        case "Sign":
                            try
                            {
                                tempList.AddLast(new Token { Name = Convert.ToString(Math.Sign(a)), Type = TokenType.Number, Priority = -1 });
                            }
                            catch (Exception e) { throw e; }
                            break;
                        default:
                            throw new Exception("при попытке подсчета выражения возникла неизвестная функция \"" + token.Name + "\"");
                    }
                }
            }

            if (tempList.Count > 1)
                throw new Exception("при попытке подсчета выражения возникла неизвестная проблема");
            return Convert.ToDouble(tempList.Last.Value.Name);
        }
    }
}