echo Processing Lexer.cs file...
perl -pibak -e "s/\} catch \(System.Exception REMPLACER\) \{/} finally {/g" Lexer.cs
perl -pibak -e "s/^\/\/package pddl4j\.lexer;$/#warning Warnings disabled!!\n#pragma warning disable 0162 \/\/ Unreachable code detected\n#pragma warning disable 0168 \/\/ Variable declared but never used\n#pragma warning disable 0169 \/\/ Private variable never used\n\nnamespace PDDLParser.Parser.Lexer\n{\n\nusing System;\nusing System.IO;\nusing System.Text;/g" Lexer.cs
perl -pibak -e "s/static \/\*final\*\//static readonly/g" Lexer.cs
perl -pibak -e "s/ instanceof / is /g" Lexer.cs
perl -pibak -e "s/^\}$/}\n}/g" Lexer.cs
perl -pibak -e "s/Throwable/System.Exception/g" Lexer.cs
perl -pibak -e "s/Integer\.parseInt/int.Parse/g" Lexer.cs

echo Processing LexerTreeConstants.cs file...
perl -pibak -e "s/^package pddl4j.lexer;$/namespace PDDLParser.Parser.Lexer\n{/g" LexerTreeConstants.cs
perl -pibak -e "s/^\}$/}\n}/g" LexerTreeConstants.cs
perl -pibak -e "s/interface/class/g" LexerTreeConstants.cs
perl -pibak -e "s/ int / const int /g" LexerTreeConstants.cs
perl -pibak -e "s/String\[\]/static string[]/g" LexerTreeConstants.cs

echo Processing LexerConstants.cs file...
perl -pibak -e "s/^using System;/namespace PDDLParser.Parser.Lexer\n{\n\nusing System;\n/g" LexerConstants.cs
perl -pibak -e "s/^\}$/}\n}/g" LexerConstants.cs

echo Processing LexerTokenManager.cs file...
perl -pibak -e "s/^public class/#warning Warnings disabled!!\n#pragma warning disable 0164 \/\/ Label not referenced\n#pragma warning disable 0168 \/\/ Variable declared but never used\n\nnamespace PDDLParser.Parser.Lexer\n{\nusing System;\nusing System.Text;\n\npublic class/g" LexerTokenManager.cs 
perl -pibak -e "s/StringBuffer/StringBuilder/g" LexerTokenManager.cs 
perl -pibak -e "s/String(\W)/string$1/g" LexerTokenManager.cs 
perl -pibak -e "s/append/Append/g" LexerTokenManager.cs
rem perl -pibak -e "s/\(active0 & 0x8000000000000120L\)/(active0 & 0x00000120L) != 0L || ((active0 >> 32) & 0x80000000L)/g" LexerTokenManager.cs
rem perl -pibak -e "s/0xffffffffffffe001L/unchecked ((long)0xffffffffffffe001L)/g" LexerTokenManager.cs
echo } >> LexerTokenManager.cs
