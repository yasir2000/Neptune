echo Processing JJ file...
perl -pibak -e "s/^(package pddl4j\.lexer;)$/\/\/$1/g" Lexer.jj
perl -pibak -e "s/^import .*$//g" Lexer.jj
perl -pibak -e "s/implements LexerTreeConstants//g" Lexer.jj
perl -pibak -e "s/file\.getName\(\)/file/g" Lexer.jj
perl -pibak -e "s/(\W)File(\W)/$1string$2/g" Lexer.jj
perl -pibak -e "s/static final/static \/\*final\*\//g" Lexer.jj
perl -pibak -e "s/boolean/bool/g" Lexer.jj
perl -pibak -e "s/(\W)String(\W)/$1string$2/g" Lexer.jj
perl -pibak -e "s/throws .* \{/{/g" Lexer.jj
perl -pibak -e "s/equals/Equals/g" Lexer.jj
perl -pibak -e "s/getClass\(\)/GetType()/g" Lexer.jj
perl -pibak -e "s/(\w+)\.class/typeof($1)/g" Lexer.jj
perl -pibak -e "s/split\(\" \"\)/Split(' ')/g" Lexer.jj
perl -pibak -e "s/length(\(\))?/Length/g" Lexer.jj
perl -pibak -e "s/substring/Substring/g" Lexer.jj
perl -pibak -e "s/message = e\.getMessage\(\)\.split\(\"\.  \"\)\[1\];/message = System.Text.RegularExpressions.Regex.Split(e.Message, \".  \")[1];/g" Lexer.jj
perl -pibak -e "s/NullPointerException/NullReferenceException/g" Lexer.jj
perl -pibak -e "s/e\.printStackTrace\(\);/Console.WriteLine(e.StackTrace);/g" Lexer.jj
perl -pibak -e "s/e\.getMessage\(\)/e.Message/g" Lexer.jj
perl -pibak -e "s/StringBuffer/StringBuilder/g" Lexer.jj
perl -pibak -e "s/append/Append/g" Lexer.jj
perl -pibak -e "s/RuntimeException/ApplicationException/g" Lexer.jj
perl -pibak -e "s/\} finally \{/} catch (System.Exception REMPLACER) {/g" Lexer.jj
perl -pibak -e "s/new string\(\)/\"\"/g" Lexer.jj
perl -pibak -e "s/ JJT/ LexerTreeConstants.JJT/g" Lexer.jj
perl -pibak -e "s/LexerTreeConstants\.JJTLexerState/JJTLexerState/g" Lexer.jj
perl -pibak -e "s/(\W)Error(\W)/$1Exception$2/g" Lexer.jj
perl -pibak -e "s/toString/ToString/g" Lexer.jj