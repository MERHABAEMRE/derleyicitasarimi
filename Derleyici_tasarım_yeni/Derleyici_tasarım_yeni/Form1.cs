using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Derleyici_tasarım_yeni
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textEditor.Focus();
        }
        List<string> varsize = new List<string>(); //List tanımlanması 

        private void button1_Click(object sender, EventArgs e)
        {

            console.Text = ""; //Text string
            string sourceCode = textEditor.Text; //Panel desing 

            var tokens = Scanner(Splitter(sourceCode)); //Tokenleri yazdırma ve Split metodu
            foreach (var s in tokens)
            {
                console.Text += s.name + ", " + s.type + "\r\n"; //Tokenler boşluk ve , koyma
            }
            console.Text += "\r\n"; //boşluk ve yeni satır

            var errors = SemanticAnalyzer(tokens); //hata
            foreach (var s in errors)
            {
                textBox1.Text += s + "\r\n"; //hataların yeni satır ve boşluk
            }

            for (int i = 0; i < Lvar.Count(); i++)
            {
                if (Lvar[i].type == "sayı")
                {
                    string[] row = { Lvar[i].isim, Lvar[i].type, Convert.ToString(Lvar[i].sayi_tanim) };
                    memoryGrid.Rows.Add(row);
                }
                else if (Lvar[i].type == "yazı")
                {
                    string[] row = { Lvar[i].isim, Lvar[i].type, Convert.ToString(Lvar[i].yazi) };
                    memoryGrid.Rows.Add(row);
                }
                else if (Lvar[i].type == "bool")
                {
                    string[] row = { Lvar[i].isim, Lvar[i].type, Convert.ToString(Lvar[i].dogrumu) };
                    memoryGrid.Rows.Add(row);
                }
                else
                {
                    string[] row = { Lvar[i].isim, "tanımsız" };
                    memoryGrid.Rows.Add(row);

                }
            }

        }
        List<string> Splitter(string sourceCode)
        {
            List<string> splitSourceCode = new List<string>();
            Regex RE = new Regex(@"\d+\.\d+|\'.*\'|\w+|\(|\)|\++|-+|\*|%|,|;|&+|\|+|<=|<|>=|>|==|=|!=|!|\{|\}|\/");
            foreach (Match m in RE.Matches(sourceCode))
            {
                splitSourceCode.Add(m.Value);
            }
            return splitSourceCode;
        }

        class Token
        {
            public string name = "";
            public string type = "";

            public Token()
            {
            }

            public Token(string name, string type)
            {
                this.name = name;
                this.type = type;
            }
        }
        class deger
        {
            public string isim;
            public int sayi_tanim;
            public string yazi;
            public string type;
            public Boolean dogrumu;
        }
        public int sayac1 = 0;
        public int sayac2 = 0;
        int f = 0;
        int c = 0;
        List<deger> Lvar = new List<deger>();
        List<Token> Scanner(List<string> splitCode)
        {
            List<Token> output = new List<Token>();
            List<string> identifiers = new List<string>(new string[] { "sayi_tanimlayicisi", "string", "bool", });
            List<string> symbols = new List<string>(new string[] { "!", "|", ",", "%", "ç", "(", ")", "{", "}", ",", ";", "&&",
                                                                        "||", "<", ">", "=", "!","++","==",">=","<=","!=" });
            List<string> reservedWords = new List<string>(new string[] { "for", "while", "Isnot", "do", "return", "break", "continue", "end" });
            bool match = false;


            for (int i = 0; i < splitCode.Count; i++)
            {
                if (identifiers.Contains(splitCode[i]) && match == false)
                {
                    output.Add(new Token(splitCode[i], "veri tipi"));
                    match = true;
                }
                if (symbols.Contains(splitCode[i]) && match == false)
                {
                    output.Add(new Token(splitCode[i], "sembol"));
                    match = true;
                }
                if (reservedWords.Contains(splitCode[i]) && match == false)
                {
                    output.Add(new Token(splitCode[i], "koşullu ifade"));
                    match = true;
                }
                if (float.TryParse(splitCode[i], out _) && match == false)
                {
                    output.Add(new Token(splitCode[i], "sayı"));
                    match = true;
                }
                if (isbool(splitCode[i]) && match == false)
                {
                    output.Add(new Token(splitCode[i], "bool"));
                    match = true;
                }
                if (isValidVar(splitCode[i]) && match == false)
                {
                    output.Add(new Token(splitCode[i], "deger"));
                    match = true;
                }
                
                if (splitCode[i].StartsWith("'") && splitCode[i].EndsWith("'") && match == false)
                {
                    output.Add(new Token(splitCode[i], "yazı"));
                    match = true;
                }
                if (match == false)
                {
                    output.Add(new Token(splitCode[i], "bilinmiyor"));
                }
                match = false;
            }
            return output;

            bool isValidVar(string v)
            {
                if (v.Length >= 1)
                {
                    if (char.IsLetter(v[0]) || v[0] == '_')
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            bool isbool(string v)
            {

                if (v.Length >= 1)
                {
                    if (char.IsLetter(v[0]))
                    {
                        if (v == "true" || v == "false")
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                    return false;
                }
                return false;

            }
        }

        List<string> SemanticAnalyzer(List<Token> tokens)
        {
            List<string> errors = new List<string>();
            Token prevInput = new Token();
            Token prevInput1 = new Token();
            List<Token> prevInput2 = new List<Token>();
            List<deger> prevInput3 = new List<deger>();
            List<string> prevInput4 = new List<string>();
            List<string> prevInput5 = new List<string>();

            int islemsayisi = 0;
            int tursayisi = tokens.Count;

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].name == ";")
                {
                    islemsayisi++;
                }
            }
            int selectedRule = 0;
            for (int i = 0; i < tursayisi; i++)
            {
                selectedRule = 0;
                if (selectedRule == 0)
                {
                    if (!(f != 0 || c != 0))
                    {
                        var x = Rule1(tokens[i]);
                        if (x.StartsWith("Start"))
                        {
                            selectedRule = 1;
                        }
                        else if (x.StartsWith("Error"))
                        {
                            errors.Add(x);
                            break;
                        }
                        continue;
                    }
                    if (f == 1 || f == 2 || f == 4 || f == 3)
                    {
                        var x = Rule2(tokens[i], tokens, i);
                        if (x.StartsWith("Start"))
                        {
                            selectedRule = 2;
                        }
                        else if (x.StartsWith("Error"))
                        {
                            errors.Add(x);
                            break;
                        }
                        else
                        {
                            i = Convert.ToInt32(x);
                            f = 0;
                            prevInput1.name = "";
                            prevInput2 = new List<Token>();
                            prevInput3 = new List<deger>();
                            prevInput4 = new List<string>();
                        }
                        continue;
                    }
                    if (c == 1)
                    {
                        var x = Rule4(tokens[i], tokens, i);
                        if (x.StartsWith("Start"))
                        {
                            selectedRule = 3;
                        }
                        else if (x.StartsWith("Error"))
                        {
                            errors.Add(x);
                            break;
                        }
                        else
                        {
                            i = Convert.ToInt32(x);
                            c = 0;
                            prevInput1.name = "";
                            prevInput2 = new List<Token>();
                            prevInput3 = new List<deger>();
                            prevInput4 = new List<string>();
                            prevInput5 = new List<string>();
                            for (int j = i; j < tokens.Count; j++)
                            {
                                if (tokens[j].name == "esle") {
                                    tursayisi = j;
                                }
                            }
                        }
                        continue;
                    }
                    if (c == 2)
                    {
                        var x = Rule5(tokens[i], tokens, i);
                        if (x.StartsWith("Start"))
                        {
                            selectedRule = 4;
                        }
                        else if (x.StartsWith("Error"))
                        {
                            errors.Add(x);
                            break;
                        }
                        else
                        {
                            i = Convert.ToInt32(x);
                            c = 0;
                            prevInput1.name = "";
                            prevInput2 = new List<Token>();
                            prevInput3 = new List<deger>();
                            prevInput4 = new List<string>();
                            prevInput5 = new List<string>();
                        }
                        continue;
                    }

                }
            }
            if (islemsayisi == 0 && tokens != null)
            {
                errors.Add("syntax error");
            }
            return errors;

            string Rule1(Token input)
            {
                List<string> identifiers = new List<string>(new string[] { "sayi_tanimlayicisi", "string", "bool" });

                if (prevInput1.name == "" && input.type == "veri tipi")
                {
                    prevInput1 = input;
                    if (prevInput1.name == "sayi_tanimlayicisi")
                    {
                        f = 1;
                    }
                    if (prevInput1.name == "string")
                    {
                        f = 2;
                    }
                    if (prevInput1.name == "bool")
                    {
                        f = 3;
                    }
                    return "Start Rule 1";
                }
                else if (prevInput.name == "" && input.type == "koşullu ifade")
                {
                    prevInput = input;
                    if (prevInput.name == "Isnot")
                    {
                        c = 1;
                    }
                    if (prevInput.name == "while")
                    {
                        c = 2;
                    }
                    if (prevInput.name == "for")
                    {
                        c = 3;
                    }
                    return "Start Rule 1";
                }
                else if (input.type == "deger" && f != 1)
                {
                    for (int i = 0; i < Lvar.Count; i++)
                    {
                        if (Lvar[i].isim == input.name)
                        {
                            f = 4;
                            prevInput2.Add(input);
                            Lvar.RemoveAt(i);
                            deger dg = new deger();
                            dg.isim = input.name;
                            Lvar.Add(dg);
                            return "Start Rule 1";
                        }
                    }
                    return "Error deger tanımlı değil Rule 1";
                }
                else if (f != 0 || c != 0)
                {
                    return "Ok Rule 1";

                } else if (input.name == "}") {
                    return "Start Rule 1";
                }

                return "Error Expected 'veri tipi' Rule 1";

            }


            string Rule2(Token input, List<Token> token, int islemim)
            {
                List<string> operators = new List<string>(new string[] { "!", "ç", ",", "|" });

                if (f == 1 || f == 2 || f == 3)
                {
                    if (input.type == "deger")
                    {
                        for (int i = 0; i < Lvar.Count; i++)
                        {
                            if (input.name == Lvar[i].isim)
                            {
                                prevInput2.Add(input);
                                Lvar.RemoveAt(i);
                                deger dg = new deger();
                                dg.isim = input.name;
                                Lvar.Add(dg);
                                return "Start Rule 2";
                            }
                        }

                        prevInput2.Add(input);
                        deger dg1 = new deger();
                        dg1.isim = input.name;
                        Lvar.Add(dg1);
                        return "Start Rule 2";
                    }
                    else if (input.type == "sembol")
                    {
                        if (input.name == "=")
                        {
                            var state = Rule3(token, islemim);
                            if (state.StartsWith("Error"))
                            {
                                errors.Add(state);
                            }
                            return state;

                        }
                        else if (input.name == ",")
                        {
                            return "Start Rule 2";
                        }

                    }
                }
                else if (f == 4)
                {
                    if (input.type == "sembol")
                    {
                        if (input.name == "=")
                        {
                            var state = Rule3(token, islemim);
                            if (state.StartsWith("Error"))
                            {
                                errors.Add(state);
                            }
                            return state;

                        }
                        else
                        {
                            return "Error syntax hatası 2";
                        }

                    }
                    else
                    {
                        return "Error syntax hatası 2";
                    }


                }

                return "Start Rule 2";

            }

            string Rule3(List<Token> token, int islemim)
            {
                List<string> operators = new List<string>(new string[] { "!", "ç", ",", "|" });
                int sayac3 = 0;

                for (int i = islemim; i < token.Count; i++)
                {
                    if (token[i].name == "=")
                    {
                        sayac1 = i + 1;
                        break;
                    }
                }


                for (int i = sayac1; i < token.Count; i++)
                {
                    if (token[i].type == "deger")
                    {
                        for (int j = 0; j < Lvar.Count; j++)
                        {
                            if (token[i].name == Lvar[j].isim && Lvar[j].type != null)
                            {
                               
                                if (token[i + 1].name == "!" || token[i + 1].name == "ç" || token[i + 1].name == "," || token[i + 1].name == "|" || token[i + 1].name == ";")
                                {

                                    if (Lvar[j].type == "sayı")
                                    {
                                        deger dg = new deger();
                                        dg.type = "sayı";
                                        dg.sayi_tanim = Lvar[j].sayi_tanim;
                                        prevInput3.Add(dg);
                                    }
                                    else if (Lvar[j].type == "yazı")
                                    {
                                        deger dg = new deger();
                                        dg.type = "yazı";
                                        dg.yazi = Lvar[j].yazi;
                                        prevInput3.Add(dg);
                                    }
                                    else if (Lvar[j].type == "bool")
                                    {
                                        deger dg = new deger();
                                        dg.type = "bool";
                                        dg.dogrumu = Lvar[j].dogrumu;
                                        prevInput3.Add(dg);
                                    }

                                }

                            }
                            else if (token[i].name == Lvar[j].isim && Lvar[j].type == null)
                            {
                                return "Error Değişken tanımlı değil Rule 3";
                            }
                        }

                    }
                    else if (operators.Contains(token[i].name))
                    {
                        if (token[i + 1].type == "deger" || token[i + 1].type == "sayı" || token[i + 1].type == "yazı")
                        {
                            prevInput4.Add(token[i].name);
                        }
                        else
                        {
                            return "Error islem hatası Rule 3";

                        }
                    }
                    else if (token[i].type == "sayı" || token[i].type == "yazı" || token[i].type == "bool")
                    {
                        if (token[i].type == "sayı")
                        {
                            deger dg = new deger();
                            dg.type = "sayı";
                            dg.sayi_tanim = Convert.ToInt32(token[i].name);
                            prevInput3.Add(dg);
                        }
                        else if (token[i].type == "yazı")
                        {
                            deger dg = new deger();
                            dg.type = "yazı";
                            dg.yazi = token[i].name;
                            prevInput3.Add(dg);
                        }
                        else if (token[i].type == "bool")
                        {
                            deger dg = new deger();
                            dg.type = "bool";
                            if (token[i].name == "true")
                            {
                                dg.dogrumu = true;
                            }
                            else {
                                dg.dogrumu = false;
                            }
                            prevInput3.Add(dg);
                        }
                    }
                    if (token[i + 1].name == ";")
                    {
                        sayac3 = i + 1;
                        string c = islem(prevInput3, prevInput4);


                        for (int j = 0; j < Lvar.Count; j++)
                        {
                            for (int k = 0; k < prevInput2.Count; k++)
                            {
                                if (Lvar[j].isim == prevInput2[k].name)
                                {
                                    try
                                    {
                                        if (c.Length >= 1)
                                        {
                                            if (char.IsLetter(c[0]))
                                            {
                                                if (c == "true" || c == "false")
                                                {
                                                    if (c == "true")
                                                    {
                                                        Lvar[j].dogrumu = true;
                                                        Lvar[j].type = "bool";
                                                    }
                                                    else
                                                    {
                                                        Lvar[j].dogrumu = false;
                                                        Lvar[j].type = "bool";
                                                    }
                                                }
                                                else {
                                                    Lvar[j].yazi = c;
                                                    Lvar[j].type = "yazı";
                                                }
                                            }
                                            else
                                            {
                                                Lvar[j].sayi_tanim = Convert.ToInt32(c);
                                                Lvar[j].type = "sayı";
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        Lvar[j].yazi = c;
                                        Lvar[j].type = "yazı";
                                    }
                                }
                            }
                        }
                        break;
                    }

                }
                return Convert.ToString(sayac3);
            }

            string Rule4(Token input, List<Token> token, int islemim)
            {
                List<string> symbols = new List<string>(new string[] { "&&", "||", "==", ">=", "<=", "!=" });
                try
                {
                    if (token[islemim].name == "(" && token[islemim].type == "sembol")
                    {
                        islemim++;

                        for (int i = islemim; i < token.Count; i++)
                        {
                            if (token[i].name != ")")
                            {
                                if (token[i].type == "deger")
                                {

                                    for (int j = 0; j < Lvar.Count; j++)
                                    {
                                        if (token[i].name == Lvar[j].isim && Lvar[j].type != null)
                                        {
                                            deger dg = new deger();
                                            dg.type = "sayı";
                                            dg.sayi_tanim = Lvar[j].sayi_tanim;
                                            prevInput3.Add(dg);
                                            break;
                                        }
                                        else if (token[islemim].name == Lvar[j].isim && Lvar[j].type == null)
                                        {
                                            return "Error if içi değer boş döndü 4";
                                        }
                                    }
                                }
                                else if (token[i].type == "sayı")
                                {
                                    deger dg = new deger();
                                    dg.type = "sayı";
                                    dg.sayi_tanim = Convert.ToInt32(token[i].name);
                                    prevInput3.Add(dg);
                                }
                                else if (token[i].type == "sembol")
                                {
                                    if (symbols.Contains(token[i].name))
                                    {
                                        prevInput5.Add(token[i].name);
                                    }
                                    else
                                    {

                                        return "Error sembol hatası döndü 4";
                                    }
                                }
                                else if (token[i].type == "bool")
                                {
                                    deger dg = new deger();
                                    dg.type = "bool";
                                    if (token[i].name == "true")
                                    {
                                        dg.dogrumu = true;
                                    }
                                    else {
                                        dg.dogrumu = false;
                                    }
                                    prevInput3.Add(dg);

                                }
                            }
                            else
                            {
                                Boolean a = Operatorislem(prevInput3, prevInput5);
                                islemim = i + 1;

                                if (a == false)
                                {
                                    if (token[islemim].name == "{" && token[islemim].type == "sembol")
                                    {
                                        return Convert.ToString(islemim);
                                    }
                                }
                                else if (a == true)
                                {
                                    for (int j = islemim; j < token.Count; j++)
                                    {
                                        if (token[j].name == "esle") {
                                            j++;

                                            if (token[j].name == "{" && token[j].type == "sembol")
                                            {
                                                return Convert.ToString(j);
                                            }
                                        }
                                    }
                                    return Convert.ToString(token.Count);
                                }

                            }
                        }


                    }
                }
                catch (Exception)
                {

                    return "Error if hatası Rule 3";
                }
                return "Error if hatası Rule 3";

            }


            string Rule5(Token input, List<Token> token, int islemim)
            {
                List<string> symbols = new List<string>(new string[] { "&&", "||", "==", ">=", "<=", "!=" });
                try
                {
                    if (token[islemim].name == "(" && token[islemim].type == "sembol")
                    {
                        islemim++;

                        for (int i = islemim; i < token.Count; i++)
                        {
                            if (token[i].name != ")")
                            {
                                if (token[i].type == "deger")
                                {

                                    for (int j = 0; j < Lvar.Count; j++)
                                    {
                                        if (token[i].name == Lvar[j].isim && Lvar[j].type != null)
                                        {
                                            deger dg = new deger();
                                            dg.type = "sayı";
                                            dg.sayi_tanim = Lvar[j].sayi_tanim;
                                            prevInput3.Add(dg);
                                            break;
                                        }
                                        else if (token[islemim].name == Lvar[j].isim && Lvar[j].type == null)
                                        {
                                            return "Error if içi değer boş döndü 4";
                                        }
                                    }
                                }
                                else if (token[i].type == "sayı")
                                {
                                    deger dg = new deger();
                                    dg.type = "sayı";
                                    dg.sayi_tanim = Convert.ToInt32(token[i].name);
                                    prevInput3.Add(dg);
                                }
                                else if (token[i].type == "sembol")
                                {
                                    if (symbols.Contains(token[i].name))
                                    {
                                        prevInput5.Add(token[i].name);
                                    }
                                    else
                                    {

                                        return "Error sembol hatası döndü 4";
                                    }
                                }
                                else if (token[i].type == "bool")
                                {
                                    deger dg = new deger();
                                    dg.type = "bool";
                                    if (token[i].name == "true")
                                    {
                                        dg.dogrumu = true;
                                    }
                                    else
                                    {
                                        dg.dogrumu = false;
                                    }
                                    prevInput3.Add(dg);

                                }
                            }
                            else
                            {
                                Boolean a = Operatorislem(prevInput3, prevInput5);
                                islemim = i + 1;

                                if (a == false)
                                {
                                    if (token[islemim].name == "{" && token[islemim].type == "sembol")
                                    {
                                        return Convert.ToString(islemim);
                                    }
                                }
                                else if (a == true)
                                {
                                    for (int j = islemim; j < token.Count; j++)
                                    {
                                        if (token[j].name == "esle")
                                        {
                                            j++;

                                            if (token[j].name == "{" && token[j].type == "sembol")
                                            {
                                                return Convert.ToString(j);
                                            }
                                        }
                                    }
                                    return Convert.ToString(token.Count);
                                }

                            }
                        }


                    }
                }
                catch (Exception)
                {

                    return "Error if hatası Rule 3";
                }
                return "Error if hatası Rule 3";

            }


            string islem(List<deger> prevInp3, List<string> prevInp4)
            {
                int x = 0;
                string x1 = "";
                int sayac1 = prevInp4.Count;
                int sayac2 = prevInp3.Count;
                List<deger> prevInp33 = new List<deger>();
                List<string> prevInp44 = new List<string>();
                f = 0;
                if (prevInp4.Count != 0)
                {

                    if (prevInp4.Count == 1)
                    {
                        if (prevInp3[0].type == "sayı" && prevInp3[1].type == "sayı")
                        {

                            if (prevInp4[0] == "ç")
                            {
                                x1 = Convert.ToString(prevInp3[0].sayi_tanim * prevInp3[1].sayi_tanim);
                            }
                            else if (prevInp4[0] == ",")
                            {
                                x1 = Convert.ToString(prevInp3[0].sayi_tanim - prevInp3[1].sayi_tanim);
                            }
                            else if (prevInp4[0] == "!")
                            {
                                x1 = Convert.ToString(prevInp3[0].sayi_tanim + prevInp3[1].sayi_tanim);
                            }
                            else if (prevInp4[0] == "|")
                            {
                                x1 = Convert.ToString(prevInp3[0].sayi_tanim / prevInp3[1].sayi_tanim);
                            }
                        }
                        else
                        {
                            if (prevInp4[0] == "!")
                            {
                                x1 = prevInp3[0].yazi + prevInp3[1].yazi;
                            }
                            else
                            {
                                return "Error islem hatası string";
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < prevInp4.Count; i++)
                        {
                            if (prevInp4[i] == "ç")
                            {
                                prevInp3[i + 1].sayi_tanim = prevInp3[i].sayi_tanim * prevInp3[i + 1].sayi_tanim;
                                if (i + 1 == prevInp3.Count - 1)
                                {
                                    prevInp33.Add(prevInp3[i + 1]);
                                }
                            }
                            else if (prevInp4[i] == "|")
                            {
                                prevInp3[i + 1].sayi_tanim = prevInp3[i].sayi_tanim / prevInp3[i + 1].sayi_tanim;
                                if (i + 1 == prevInp3.Count - 1)
                                {
                                    prevInp33.Add(prevInp3[i + 1]);
                                }
                            }
                            else
                            {
                                prevInp44.Add(prevInp4[i]);
                                prevInp33.Add(prevInp3[i]);
                                if (i + 1 == prevInp3.Count - 1)
                                {
                                    prevInp33.Add(prevInp3[i + 1]);
                                }
                            }

                        }

                        for (int i = 0; i < prevInp44.Count; i++)
                        {
                            if (prevInp44[i] == ",")
                            {
                                prevInp33[i + 1].sayi_tanim = prevInp33[i].sayi_tanim - prevInp33[i + 1].sayi_tanim;
                            }
                            else if (prevInp44[i] == "!")
                            {
                                prevInp33[i + 1].sayi_tanim = prevInp33[i].sayi_tanim + prevInp33[i + 1].sayi_tanim;
                            }
                        }
                        x1 = Convert.ToString(prevInp33[prevInp33.Count - 1].sayi_tanim);
                        return x1;
                    }
                }
                else
                {
                    try
                    {
                        if (prevInp3[0].type == "sayı")
                        {
                            return Convert.ToString(prevInp3[0].sayi_tanim);
                        }
                        if (prevInp3[0].type == "bool")
                        {
                            return Convert.ToString(prevInp3[0].dogrumu);
                        }
                        if (prevInp3[0].type == "yazı")
                        {
                            return Convert.ToString(prevInp3[0].yazi);
                        }
                    }
                    catch (Exception e)
                    {
                        return "Error islem hatası string";
                    }

                    if (prevInp3[0].type == "bilinmiyor")
                    {
                        return "Error islem hatası string";
                    }
                    else
                    {
                        return Convert.ToString(prevInp3[0].yazi);

                    }
                }

                return Convert.ToString(x1);
            }

            Boolean Operatorislem(List<deger> prevInp3, List<string> prevInp5)
            {
                List<string> symbols = new List<string>(new string[] { "&&", "||", "==", ">=", "<=", "!=" });
                List<Boolean> prevInp33 = new List<Boolean>();
                List<string> prevInp55 = new List<string>();
                Boolean a;
                int sayac4 = 0;

                if (prevInp5.Count != 0)
                {
                    for (int i = 0; i < prevInp5.Count; i++)
                    {
                            if (prevInp5[i] == "==")
                            {
                                if (prevInp3[sayac4].sayi_tanim == prevInp3[sayac4 + 1].sayi_tanim)
                                {
                                    prevInp33.Add(true);
                                    sayac4 += 2;
                                }
                                else
                                {
                                    prevInp33.Add(false);
                                    sayac4 += 2;
                                }
                            }
                            else if (prevInp5[i] == ">=")
                            {
                                if (prevInp3[sayac4].sayi_tanim >= prevInp3[sayac4 + 1].sayi_tanim)
                                {
                                    prevInp33.Add(true);
                                    sayac4 += 2;
                                }
                                else
                                {
                                    prevInp33.Add(false);
                                    sayac4 += 2;
                                }
                            }
                            else if (prevInp5[i] == "<=")
                            {
                                if (prevInp3[sayac4].sayi_tanim <= prevInp3[sayac4 + 1].sayi_tanim)
                                {
                                    prevInp33.Add(true);
                                    sayac4 += 2;
                                }
                                else
                                {
                                    prevInp33.Add(false);
                                    sayac4 += 2;
                                }
                            }
                            else if (prevInp5[i] == "!=")
                            {
                                if (prevInp3[sayac4].sayi_tanim != prevInp3[sayac4 + 1].sayi_tanim)
                                {
                                    prevInp33.Add(true);
                                    sayac4 += 2;
                                }
                                else
                                {
                                    prevInp33.Add(false);
                                    sayac4 += 2;
                                }
                            }
                            else if (prevInp5[i] == "&&" || prevInp5[i] == "||")
                            {
                                prevInp55.Add(prevInp5[i]);
                            }
                        
                    }
                }else if(prevInp3[0].type == "bool") {

                    return prevInp3[0].dogrumu;
                }

                if (prevInp55.Count != 0)
                {
                    for (int i = 0; i < prevInp55.Count; i++)
                    {
                        if (prevInp55[i] == "&&")
                        {
                            if (prevInp33[i] && prevInp33[i + 1])
                            {
                                prevInp33[i + 1] = true;
                            }
                            else
                            {
                                prevInp33[i + 1] = false;
                            }
                        }
                        else if (prevInp55[i] == "||")
                        {
                            if (prevInp33[i] || prevInp33[i + 1])
                            {
                                prevInp33[i + 1] = true;
                            }
                            else
                            {
                                prevInp33[i + 1] = false;
                            }
                        }
                    }
                }
                else
                {
                    return prevInp33[0];
                }


                return prevInp33[prevInp33.Count-1];
            }

        }
    }
}
