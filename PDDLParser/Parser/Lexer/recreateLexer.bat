@echo off
call jjtree Lexer.jjt
call replaceJJ
call java -jar csjavacc.jar Lexer.jj
del LexerTreeConstants.cs
rename LexerTreeConstants.java LexerTreeConstants.cs
del *.java
call replaceCS
del *.*bak
echo Done!