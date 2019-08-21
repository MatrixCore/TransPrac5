  // Do learn to insert your names and a brief description of what the program is supposed to do!

  // This is a skeleton program for developing a parser for Modula-2 declarations
  // P.D. Terry, Rhodes University
  using Library;
  using System;
  using System.Text;

  class Token {
    public int kind;
    public string val;

    public Token(int kind, string val) {
      this.kind = kind;
      this.val = val;
    }

  } // Token

  class Declarations {

    // +++++++++++++++++++++++++ File Handling and Error handlers ++++++++++++++++++++

    static InFile input;
    static OutFile output;

    static string NewFileName(string oldFileName, string ext) {
    // Creates new file name by changing extension of oldFileName to ext
      int i = oldFileName.LastIndexOf('.');
      if (i < 0) return oldFileName + ext; else return oldFileName.Substring(0, i) + ext;
    } // NewFileName

    static void ReportError(string errorMessage) {
    // Displays errorMessage on standard output and on reflected output
      Console.WriteLine(errorMessage);
      output.WriteLine(errorMessage);
    } // ReportError

    static void Abort(string errorMessage) {
    // Abandons parsing after issuing error message
      ReportError(errorMessage);
      output.Close();
      System.Environment.Exit(1);
    } // Abort

    // +++++++++++++++++++++++  token kinds enumeration +++++++++++++++++++++++++

    const int //Added the additional symbols 
      noSym        = 0,
      EOFSym       = 1,
      periodSym    = 2,
      commaSym     = 3,
      dPeriodSym   = 4,
      numSym       = 5,
      scSym        = 6,
      colonSym     = 7,
      eqlSym       = 8,
      charSym      = 9,
      idSym        = 10,
      endSym       = 11,
      ofSym        = 12,
      arraySym     = 13,
      pointSym     = 14,
      recSym       = 15,
      setSym       = 16,
      toSym        = 17,
      typeSym      = 18,
      varSym       = 19,
      lsbSym       = 20, 
      rsbSym       = 21,
      lparenSym    = 22,
      rparenSym    = 23,
      lcbSym       = 24,
      rcbSym       = 25,
      comSym       = 26;


    // and others like this

    // +++++++++++++++++++++++++++++ Character Handler ++++++++++++++++++++++++++

    const char EOF = '\0';
    static bool atEndOfFile = false;

    // Declaring ch as a global variable is done for expediency - global variables
    // are not always a good thing

    static char ch;    // look ahead character for scanner

    static void GetChar() {
    // Obtains next character ch from input, or CHR(0) if EOF reached
    // Reflect ch to output
      if (atEndOfFile) ch = EOF;
      else {
        ch = input.ReadChar();
        atEndOfFile = ch == EOF;
        if (!atEndOfFile) output.Write(ch);
      }
    } // GetChar

    // +++++++++++++++++++++++++++++++ Scanner ++++++++++++++++++++++++++++++++++

    // Declaring sym as a global variable is done for expediency - global variables
    // are not always a good thing

    static Token sym; 
    static void GetSym() {
        // Scans for next sym from input
        while (ch > EOF && ch <= ' ') GetChar();
        StringBuilder symLex = new StringBuilder();
        int symKind = noSym;
    //The following if, if else, amd else statements are the foundations of this scanner

    if (char.IsDigit(ch)) //First if statement checks if the first char is a digit. If it is, then it has to be a number symbol. 
    {
        while (char.IsDigit(ch)) { symLex.Append(ch); GetChar(); } //While loop builds up the numSym
        symKind = numSym;
        
    }
    else if (char.IsLetter(ch)) //This else if statement deals with identifyers and reserved keywords
    {
        while (char.IsDigit(ch) || char.IsLetter(ch)) { symLex.Append(ch); GetChar(); } 
        // Because our scanner allows both numbers AND digits WITHIN (not beginning) 
        // the identifier, this while loop builds up the potential identifier
        switch (symLex.ToString())                                                     
        {
            case "END":
                symKind = endSym;
                break;
            case "OF":
                symKind = ofSym;
                break;
            case "ARRAY":
                symKind = arraySym;
                break;
            case "POINTER":
                symKind = pointSym;
                break;
            case "RECORD":
                symKind = recSym;
                break;
            case "SET":
                symKind = setSym;
                break;
            case "TO":
                symKind = toSym;
                break;
            case "TYPE":
                symKind = typeSym;
                break;
            case "VAR":
                symKind = varSym;
                break;
            default:
                symKind = idSym;
                break;
        }
    }

    else //Finally we've hit the else statement containing all the other symbols, most of which are one character long.
    {
        switch (ch)
        {
            case ',':
                symKind = commaSym;
                symLex.Append(ch);
                GetChar();
                break;
            case '.':
                symLex.Append(ch);
                GetChar();
                if (ch == '.') { symKind = dPeriodSym; symLex.Append(ch); GetChar(); break;}
                //Seeing as a '.' could be a period symbol or a double period symbol, 
                //this if statement checks one char forwardto see which one it is.
                else symKind = periodSym;
                break;
            case ':':
                symKind = colonSym;
                symLex.Append(ch);
                GetChar();
                break;
            case ';':
                symKind = scSym;
                symLex.Append(ch);
                GetChar();
                break;
            case '=':
                symKind = eqlSym;
                symLex.Append(ch);
                GetChar();
                break;
            case '[':
                symKind = lsbSym;
                symLex.Append(ch);
                GetChar();
                break;
            case ']':
                symKind = rsbSym;
                symLex.Append(ch);
                GetChar();
                break;
            case '(':
                symLex.Append(ch);
                GetChar();
                if (ch=='*')
                {
                    while(true)
                    {
                        symLex.Append(ch);
                        GetChar();
                        if(ch=='*')
                        {
                            GetChar(); 
                            if(ch==')') symLex.Append(ch); symKind = comSym; GetChar(); break;
                            //Not knowing whether to completly ignore the comments or make them
                            //a symbol, I decided to make it a symbol to make testing clearer.
                        }
                    }

                }
                symKind = lparenSym;
                break;
            case ')':
                symKind = rparenSym;
                symLex.Append(ch);
                GetChar();
                break;
            case '{':
                symKind = lcbSym;
                symLex.Append(ch);
                GetChar();
                break;
            case '}':
                symKind = rcbSym;
                symLex.Append(ch);
                GetChar();
                break;
            case '\0' :
                symKind = EOFSym;
                symLex.Append(ch);                
                break;
            default:
                symKind = noSym;
                GetChar();
                break;
        }
    }
      // over to you!

      sym = new Token(symKind, symLex.ToString());
    } // GetSym

    // +++++++++++++++++++++++++++++++ Parser +++++++++++++++++++++++++++++++++++

    static void Accept(int wantedSym, string errorMessage) {
    // Checks that lookahead token is wantedSym
      if (sym.kind == wantedSym) GetSym(); else Abort(errorMessage);
    } // Accept

    static void Accept(IntSet allowedSet, string errorMessage) {
    // Checks that lookahead token is in allowedSet
      if (allowedSet.Contains(sym.kind)) GetSym(); else Abort(errorMessage);
    } // Accept

    static void Mod2Decl()
    {
        IntSet temp = new IntSet(typeSym, varSym);
        while (temp.Contains(sym.kind)) // kind of TypeDecl or VarDec
        {
            Declaration();
        }
    }

    static void Declaration()
    {
        switch (sym.kind)
        {
            case typeSym:
                Accept(typeSym, "Expected TYPE");
                while(sym.kind == idSym)
                {
                    TypeDecl();
                    Accept(scSym, "Expected semi colon - Declaration");
                }
                break;

            case varSym:
                Accept(varSym, "Expected VAR");
                while(sym.kind == varSym)
                {
                    VarDec1();
                    Accept(scSym, "Expected semi colon - Declaration");
                }
                break;

            default:
                Abort("Invalid declaration - Declaration");
                break;
        }
    }

    static void TypeDecl()
    {
        Accept(idSym, "Missing identifer - TypeDec");
        Accept(eqlSym, "Missing equals - TypeDec");
        Type();
    }

    static void VarDec1()
    {
        IdentList();
        Accept(colonSym, "Missing colon - VarDec");
        Type();
    }

    static void Type()
    {
        switch (sym.kind)
        {
            case idSym:
                SimpleType();
                break;

            case lparenSym:
                SimpleType();
                break;

            case lsbSym:
                SimpleType();
                break;

            case arraySym:
                ArrayType();
                break;

            case recSym:
                RecordType();
                break;

            case setSym:
                SetType();
                break;

            case pointSym:
                PointerType();
                break;

            default:
                Abort("Invalid type - Type");
                break;
        }
    }

    static void SimpleType()
    {
        switch (sym.kind)
        {
            case idSym:
                QualIdent();
                if (sym.kind == lsbSym)
                {
                    Subrange();
                }
                break;

            case lsbSym:
                Subrange();
                break;

            case lparenSym:
                Enumeration();
                break;

            default:
                Abort("excuse me, why");
                break;
        }
    }

    static void QualIdent()
    {
        Accept(idSym, "Identifier expected - Qualident");
        while(sym.kind == periodSym)
        {
            Accept(periodSym, "Period expected - QualIdent");
            Accept(idSym, "Identifer expected - QualIdent");
        }
    }

    static void Subrange()
    {
        Accept(lsbSym, "[ expected - Subrange");
        Constant();
        Accept(dPeriodSym, ".. expected - Subrange");
        Constant();
        Accept(rsbSym, "] expected - Subrange");
    }

    static void Constant()
    {
        if (sym.kind == numSym)
        {
            Accept(numSym, "Number expected - Constant");
        }
        else if(sym.kind == idSym)
        {
            Accept(idSym, "Identifer expected - Constant");
        }
    }

    static void Enumeration()
    {
        Accept(rcbSym, "( expected - Enumeration");
        IdentList();
        Accept(lcbSym, ") expected - Enumeration");
    }

    static void IdentList()
    {
        Accept(idSym, "Identifier expected - IdentList");
        while(sym.kind == commaSym)
        {
            Accept(commaSym, ", is expected - IdentList");
            Accept(idSym, "Identifier expected - IdentList");
        }
    }

    static void ArrayType()
    {
        Accept(arraySym, "Expected key word ARRAY");
        SimpleType();
        while (sym.kind == commaSym)
        {
            Accept(commaSym, "Comma expected - ArrayType");
            SimpleType();
        }
        Accept(ofSym, "OF expected");
        Type();
    }

    static void RecordType()
    {
        Accept(recSym, "RECORD expected");
        FieldList();
        Accept(endSym, "END expected");
    }

    static void FieldLists()
    {
        FieldList();
        while(sym.kind == commaSym)
        {
            Accept(commaSym, "Comma expected - FieldLists");
            FieldList();
        }
    }

    static void FieldList()
    {
        if (sym.kind == idSym)
        {
            IdentList();
            Accept(colonSym, "Colon expected - FieldList");
            Type();
        }
    }

    static void SetType()
    {
        Accept(setSym, "SET is expected");
        Accept(ofSym, "OF is expected");
        SimpleType();
    }

    static void PointerType()
    {
        Accept(pointSym, "POINTER is expected");
        Accept(toSym, "TO is expected");
        Type();
    }

    // +++++++++++++++++++++ Main driver function +++++++++++++++++++++++++++++++

    public static void Main(string[] args) {
      // Open input and output files from command line arguments
      if (args.Length == 0) {
        Console.WriteLine("Usage: Declarations FileName");
        System.Environment.Exit(1);
      }
      input = new InFile(args[0]);
      output = new OutFile(NewFileName(args[0], ".out"));

      GetChar();                                  // Lookahead character

        //  To test the scanner we can use a loop like the following:
        /*
      do {
        GetSym();                                 // Lookahead symbol
        OutFile.StdOut.Write(sym.kind, 3);
        OutFile.StdOut.WriteLine(" " + sym.val);  // See what we got
      } while (sym.kind != EOFSym);
    */  

    //After the scanner is debugged we shall substitute this code:

      GetSym();                                   // Lookahead symbol
      Mod2Decl();                                 // Start to parse from the goal symbol
      // if we get back here everything must have been satisfactory
      Console.WriteLine("Parsed correctly");

      output.Close();
    } // Main

  } // Declarations
