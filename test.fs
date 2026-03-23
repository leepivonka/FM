\ FM FORTH tests
\ Some test adapted from https://forth-standard.org

Hex  \ numbers in hex
PSP-Reset  \ clear parm stack

: IsEmpty ( -- )  Depth Abort" Stack not empty"  ;
: Is= ( a b -- )  <> Abort" Mismatch!" ;
: Is2= ( da db -- )  Rot <> >R <> R> Or Abort" 2Mismatch!" ;

   0 Constant 0S	IsEmpty  0S     0 Is= IsEmpty 
ffff Constant 1S	IsEmpty  1S  ffff Is= IsEmpty
8000 Constant MSB
7fff Constant MAX-INT
8000 Constant MIN-INT
   0 Constant MIN-UINT
8000 Constant MID-UINT
8001 Constant MID-UINT+1
ffff Constant MAX-UINT

\ ---- Parm Stack ----------------------------------------------

\ Header "PSP-Reset",0 ; ( ... -- )  clear parameter stack
1111 PSP-Reset IsEmpty

\ Header "Dup",0 ; ( a -- a a )
1234 Dup 1234 Is= 1234 Is= IsEmpty

\ Header "Drop", 0 ; ( a -- )
2345 Drop IsEmpty

\ Header "Swap", 0 ; ( a b -- b a )
3456 4567 Swap 4356 Is= 3456 Is= IsEmpty

\ Header "Over", 0 ; ( a b -- a b a )
5678 6789 Over 5678 Is= 6789 Is= 5678 Is= IsEmpty

\ Header "Rot", 0 ; ( a b c -- b c a )
1234 2345 3456 Rot 1234 Is= 3456 Is= 2345 Is= IsEmpty

\ Header "Nip",0 ; ( a b -- b )
4321 5432 Nip 4321 Is= IsEmpty

\ Header "Tuck",0 ; ( a b -- b a b )
1234 5678 Tuck 5678 Is= 1234 Is= 5678 Is= IsEmpty

\ Header "2Drop", 0 ; ( a b -- )
3210 2109 1098 2Drop 3210 Is= IsEmpty

\ Header "2Dup", 0 ; ( a b -- a b a b )
1234 2345 3456 2Dup 2345 3456 Is2=  2345 3456 Is2= 1234 Is= IsEmpty

\ Header "2Over", 0 ; ( a b c d -- a b c d a b )
1234 2345 3456 4567 5678 2Over 2345 3456 Is2=  4567 5678 Is2= 2345 3456 Is2=  1234 Is= IsEmpty

\ Header "2Swap", 0 ; ( a b c d -- c d a b )
1234 2345 3456 4567 5678 2Swap 2345 3456 Is2=  4567 5678 Is2=  1234 Is= IsEmpty

\ Header "Depth", 0 ; ( -- n ) number of items on parameter stack
1 2 3 Depth 3 Is= PSP-Reset

\ Header "Pick",0 ; ( xu...x1 x0 u -- xu...x1 x0 xu )
1111 2222 3333 1 Pick 2222 Is= 3333 Is= 2222 Is= 1111 Is= IsEmpty

\ -------------- Return stack ----------------------

\ Header ">R", 0 ; ( a -- ) (R: -- a )  push to return stack
\ Header "R@", 0 ; ( -- a ) (R: a -- a )  get a copy of top of return stack
\ Header "R>", 0 ; ( -- a ) (R: a -- )  pop from return stack
: TestR1 ( -- )
  11111 >R 22222 >R IsEmpty
  R@ 22222 Is= IsEmpty
  R> 22222 Is= R> 11111 Is= IsEmpty
  ;
TestR1

\ ------------ Arithmetic -------------------

\ Header "+", 0 ; ( a b -- a+b )
255 1025 + 127A Is= IsEmpty

\ Header "-", 0 ; ( a b -- a-b )
1025 255 - 0DD0 Is= IsEmpty

\ Header "Negate", 0 ; ( n -- -n )
ffe0 Negate 0020 Is= IsEmpty
00e0 Negate FF20 Is= IsEmpty

\ Header "Abs",0 ; ( n -- |n| )
ffe0 Abs 0020 Is= IsEmpty
00e0 Abs 00E0 Is= IsEmpty

\ Header "1+", 0 ; ( n -- n+1 )
1032 1+ 1033 Is= IsEmpty

\ Header "2+",0 ; ( n -- n+2 )
12ff 2+ 1301 Is= IsEmpty

\ Header "1-", 0 ; ( n -- n-1 )
0537 1- 0536 Is= IsEmpty

\ Header "2*", 0 ; ( n -- n*2 )  shift left
0537 2* 0A6E Is= IsEmpty

\ Header "DNegate",0 ; ( d1 -- -d1 )  return -d1
5678 1234 DNegate edcb Is= a988 Is= IsEmpty

\ Header "DAbs",0 ; ( d -- ud )  ud is the absolute value of d
a988 edcb DAbs 1234 Is= 5678 Is= IsEmpty
5678 1234 DAbs 1234 Is= 5678 Is= IsEmpty

\ Header "Max",0 ; ( a b -- max )  signed
   0032    1032 Max     1032 Is= IsEmpty
   0032    fedc Max     0032 Is= IsEmpty
   1234    8032 Max     1234 Is= IsEmpty
      0       1 MAX        1 Is= IsEmpty
      1       2 MAX        2 Is= IsEmpty
     -1       0 MAX        0 Is= IsEmpty
     -1       1 MAX        1 Is= IsEmpty
MIN-INT       0 MAX        0 Is= IsEmpty
MIN-INT MAX-INT MAX  MAX-INT Is= IsEmpty
      0 MAX-INT MAX  MAX-INT Is= IsEmpty
      0       0 MAX        0 Is= IsEmpty
      1       1 MAX        1 Is= IsEmpty
      1       0 MAX        1 Is= IsEmpty
      2       1 MAX        2 Is= IsEmpty
      0      -1 MAX        0 Is= IsEmpty
      1      -1 MAX        1 Is= IsEmpty
      0 MIN-INT MAX        0 Is= IsEmpty
MAX-INT MIN-INT MAX  MAX-INT Is= IsEmpty
MAX-INT       0 MAX  MAX-INT Is= IsEmpty

\ Header "Min",0 ; ( a b -- min )  signed
0032 1032 Min 0032 Is= IsEmpty
0032 fedc Min fedc Is= IsEmpty
1234 8032 Min 8032 Is= IsEmpty
      0       1 MIN        0 Is= IsEmpty
      1       2 MIN        1 Is= IsEmpty
     -1       0 MIN       -1 Is= IsEmpty
     -1       1 MIN       -1 Is= IsEmpty
MIN-INT       0 MIN  MIN-INT Is= IsEmpty
MIN-INT MAX-INT MIN  MIN-INT Is= IsEmpty
      0 MAX-INT MIN        0 Is= IsEmpty
      0       0 MIN        0 Is= IsEmpty
      1       1 MIN        1 Is= IsEmpty
      1       0 MIN        0 Is= IsEmpty
      2       1 MIN        1 Is= IsEmpty
      0      -1 MIN       -1 Is= IsEmpty
      1      -1 MIN       -1 Is= IsEmpty
      0 MIN-INT MIN  MIN-INT Is= IsEmpty
MAX-INT MIN-INT MIN  MIN-INT Is= IsEmpty
MAX-INT       0 MIN        0 Is= IsEmpty

\ Header "2/", 0 ; ( n -- n/2 ) signed shift right
0537 2/ 029B Is= IsEmpty
fedc 2/ ff6e Is= IsEmpty

\ Header "U2/",0 ; ( u -- u/2 ) unsigned shift right
0537 2/ 029B Is= IsEmpty
fedc 2/ 7f6e Is= IsEmpty

\ Header "LShift", 0 ; ( a u -- a<<u ) logical shift left
1234 4 LShift 2340 Is= IsEmpty

\ Header "RShift", 0 ; ( a u -- a>>u ) logical shift right
1234 4 RShift 0123 Is= IsEmpty

\ Header "S>D",0 ; ( n -- d )  Convert the signed number n to the double-cell number d
1234 S>D 0000 Is= 1234 Is= IsEmpty
fedc S>D ffff Is= fedc Is= IsEmpty

\ Header "UM*", 0 ; ( u1 u2 -- ud ) unsigned 16x16 -> 32-bit result
1025 255 UM* 0025 Is= A649 Is= IsEmpty
0 0 UM*  0 Is= 0 Is= IsEmpty
0 1 UM*  0 Is= 0 Is= IsEmpty
1 0 UM*  0 Is= 0 Is= IsEmpty
1 2 UM*  0 Is= 2 Is= IsEmpty
2 1 UM*  0 Is= 2 Is= IsEmpty
3 3 UM*  0 Is= 9 Is= IsEmpty
MID-UINT+1 1 RSHIFT 2 UM*  0 Is=  MID-UINT+1 Is= IsEmpty
MID-UINT+1          2 UM*  1 Is=           0 Is= IsEmpty
MID-UINT+1          4 UM*  2 Is=           0 Is= IsEmpty
        1S          2 UM*  1 Is=  1S 1 LShift Is= IsEmpty
  MAX-UINT   MAX-UINT UM*  1 Invert Is= 1 Is= IsEmpty

\ Header "M*",0 ; ( a b -- dc ) 16x16 -> 32 signed
      0       0 M*        0 S>D Is2= IsEmpty
      0       1 M*        0 S>D Is2= IsEmpty
      1       0 M*        0 S>D Is2= IsEmpty
      1       2 M*        2 S>D Is2= IsEmpty
      2       1 M*        2 S>D Is2= IsEmpty
      3       3 M*        9 S>D Is2= IsEmpty
     -3       3 M*       -9 S>D Is2= IsEmpty
      3      -3 M*       -9 S>D Is2= IsEmpty
     -3      -3 M*        9 S>D Is2= IsEmpty
      0 MIN-INT M*        0 S>D Is2= IsEmpty
      1 MIN-INT M*  MIN-INT S>D Is2= IsEmpty
      2 MIN-INT M*        0 1S  Is2= IsEmpty
      0 MAX-INT M*        0 S>D Is2= IsEmpty
      1 MAX-INT M*  MAX-INT S>D Is2= IsEmpty
      2 MAX-INT M*  MAX-INT     1 LSHIFT 0 Is2= IsEmpty
MIN-INT MIN-INT M*        0 MSB 1 RSHIFT   Is2= IsEmpty
MAX-INT MIN-INT M*      MSB MSB 2/         Is2= IsEmpty
MAX-INT MAX-INT M*        1 MSB 2/ INVERT  Is2= IsEmpty

\ Header "*", 0 ; ( a b -- a*b ) 16x16 -> 16 (low word)
1025 0014 * 42E4 Is= IsEmpty
 0  0 *   0 Is= IsEmpty
 0  1 *   0 Is= IsEmpty
 1  0 *   0 Is= IsEmpty
 1  2 *   2 Is= IsEmpty
 2  1 *   2 Is= IsEmpty
 3  3 *   9 Is= IsEmpty
-3  3 *  -9 Is= IsEmpty
 3 -3 *  -9 Is= IsEmpty
-3 -3 *   9 Is= IsEmpty
MID-UINT+1 1 RSHIFT 2 *                MID-UINT+1 Is= IsEmpty
MID-UINT+1 2 RSHIFT 4 *                MID-UINT+1 Is= IsEmpty
MID-UINT+1 1 RSHIFT MID-UINT+1 OR 2 *  MID-UINT+1 Is= IsEmpty

\ Header "UM/MOD", 0 ; ( ud u -- ur uq ) unsigned 32/16 -> 16 remainder, 16 quotient
27c0 0009 000a UM/Mod EA60 Is= 0000 Is= IsEmpty

\ Header "SM/Rem",0 ; ( d1 n1 -- n_remainder n_quotient )  Symmetric signed division
       0 S>D              1 SM/REM   0 Is=       0 Is= IsEmpty
       1 S>D              1 SM/REM   1 Is=       0 Is= IsEmpty
       2 S>D              1 SM/REM   2 Is=       0 Is= IsEmpty
      -1 S>D              1 SM/REM  -1 Is=       0 Is= IsEmpty
      -2 S>D              1 SM/REM  -2 Is=       0 Is= IsEmpty
       0 S>D             -1 SM/REM   0 Is=       0 Is= IsEmpty
       1 S>D             -1 SM/REM  -1 Is=       0 Is= IsEmpty
       2 S>D             -1 SM/REM  -2 Is=       0 Is= IsEmpty
      -1 S>D             -1 SM/REM   1 Is=       0 Is= IsEmpty
      -2 S>D             -1 SM/REM   2 Is=       0 Is= IsEmpty
       2 S>D              2 SM/REM   1 Is=       0 Is= IsEmpty
      -1 S>D             -1 SM/REM   1 Is=       0 Is= IsEmpty
      -2 S>D             -2 SM/REM   1 Is=       0 Is= IsEmpty
       7 S>D              3 SM/REM   2 Is=       1 Is= isEmpty
       7 S>D             -3 SM/REM  -2 Is=       1 Is= IsEmpty
      -7 S>D              3 SM/REM  -2 Is=       1 Is= IsEmpty
      -7 S>D             -3 SM/REM   2 Is=      -1 Is= IsEmpty
 MAX-INT S>D              1 SM/REM  MAX-INT Is=  0 Is= IsEmpty
 MIN-INT S>D              1 SM/REM  MIN-INT Is=  0 Is= IsEmpty
 MAX-INT S>D        MAX-INT SM/REM        1 Is=  0 Is= IsEmpty
 MIN-INT S>D        MIN-INT SM/REM        1 Is=  0 Is= IsEmpty
      1S 1                4 SM/REM  MAX-INT Is=  3 Is= IsEmpty
       2 MIN-INT M*       2 SM/REM  MIN-INT Is=  0 Is= IsEmpty
       2 MIN-INT M* MIN-INT SM/REM        2 Is=  0 Is= IsEmpty
       2 MAX-INT M*       2 SM/REM  MAX-INT Is=  0 Is= IsEmpty
       2 MAX-INT M* MAX-INT SM/REM        2 Is=  0 Is= IsEmpty
 MIN-INT MIN-INT M* MIN-INT SM/REM  MIN-INT Is=  0 Is= IsEmpty
 MIN-INT MAX-INT M* MIN-INT SM/REM  MAX-INT Is=  0 Is= IsEmpty
 MIN-INT MAX-INT M* MAX-INT SM/REM  MIN-INT Is=  0 Is= IsEmpty
 MAX-INT MAX-INT M* MAX-INT SM/REM  MAX-INT Is=  0 Is= IsEmpty

\ Header "/MOD", 0 ; ( n1 n2 -- rem quot ) signed division
7fff 0a /Mod 0CCC Is= 0007 Is= IsEmpty

\ Header "/", 0 ; ( n1 n2 -- quot ) signed division
89ab 1234 / fff9 Is= IsEmpty

\ ---------- Logic ---------------

\ Header "True", 0 ; ( -- true )
True ffff Is= IsEmpty

\ Header "False", 0 ; ( -- false )
False 0 Is= IsEmpty

\ Header "0=",0 ; ( n -- flag )
1 0= False Is= IsEmpty
0 0= True  Is= IsEmpty

\ Header "0<",0 ; ( n -- flag )
 1 0< False Is= IsEmpty
 0 0< False Is= IsEmpty
-1 0< True  Is= IsEmpty

\ Header "0>",0 ; ( n -- flag )
 1 0< True  Is= IsEmpty
 0 0< False Is= IsEmpty
-1 0< False Is= IsEmpty

\ Header "=", 0 ; ( a b -- flag )
1233 1234 = False Is= IsEmpty
1334 1234 = False Is= IsEmpty
1234 1234 = True  Is= IsEmpty

\ Header "<>", 0 ; ( a b -- flag )
1233 1234 <> True  Is= IsEmpty
1334 1234 <> True  Is= IsEmpty
1234 1234 <> False Is= IsEmpty

\ Header "<",0 ; ( a b -- flag ) signed
      0       1 < True  Is= IsEmpty
      1       2 < True  Is= IsEmpty
     -1       0 < True  Is= IsEmpty
     -1       1 < True  Is= IsEmpty
MIN-INT       0 < True  Is= IsEmpty
MIN-INT MAX-INT < True  Is= IsEmpty
      0 MAX-INT < True  Is= IsEmpty
      0       0 < False Is= IsEmpty
      1       1 < False Is= IsEmpty
      1       0 < False Is= IsEmpty
      2       1 < False Is= IsEmpty
      0      -1 < False Is= IsEmpty
      1      -1 < False Is= IsEmpty
      0 MIN-INT < False Is= IsEmpty
MAX-INT MIN-INT < False Is= IsEmpty
MAX-INT       0 < False Is= IsEmpty

\ Header ">",0 ; ( a b -- flag ) signed
      0       1 > False Is= IsEmpty
      1       2 > False Is= IsEmpty
     -1       0 > False Is= IsEmpty
     -1       1 > False Is= IsEmpty
MIN-INT       0 > False Is= IsEmpty
MIN-INT MAX-INT > False Is= IsEmpty
      0 MAX-INT > False Is= IsEmpty
      0       0 > False Is= IsEmpty
      1       1 > False Is= IsEmpty
      1       0 > True  Is= IsEmpty
      2       1 > True  Is= IsEmpty
      0      -1 > True  Is= IsEmpty
      1      -1 > True  Is= IsEmpty
      0 MIN-INT > True  Is= IsEmpty
MAX-INT MIN-INT > True  Is= IsEmpty
MAX-INT       0 > True  Is= IsEmpty

\ Header "U<",0 ; ( u1 u2 -- flag ) unsigned less than
       0        1 U< True  Is= IsEmpty
       1        2 U< True  Is= IsEmpty
       0 MID-UINT U< True  Is= IsEmpty
       0 MAX-UINT U< True  Is= IsEmpty
MID-UINT MAX-UINT U< True  Is= IsEmpty
       0        0 U< False Is= IsEmpty
       1        1 U< False Is= IsEmpty
       1        0 U< False Is= IsEmpty
       2        1 U< False Is= IsEmpty
MID-UINT        0 U< False Is= IsEmpty
MAX-UINT        0 U< False Is= IsEmpty
MAX-UINT MID-UINT U< False Is= IsEmpty

\ Header "U>",0 ; ( u1 u2 -- flag ) unsigned greater than
       0        1 U> False Is= IsEmpty
       1        2 U> False Is= IsEmpty
       0 MID-UINT U> False Is= IsEmpty
       0 MAX-UINT U> False Is= IsEmpty
MID-UINT MAX-UINT U> False Is= IsEmpty
       0        0 U> False Is= IsEmpty
       1        1 U> False Is= IsEmpty
       1        0 U> True  Is= IsEmpty
       2        1 U> True  Is= IsEmpty
MID-UINT        0 U> True  Is= IsEmpty
MAX-UINT        0 U> True  Is= IsEmpty
MAX-UINT MID-UINT U> True  Is= IsEmpty

\ Header "And", 0 ; ( a b -- a&b )
0        0 AND   0 Is= IsEmpty
0        1 AND   0 Is= IsEmpty
1        0 AND   0 Is= IsEmpty
1        1 AND   1 Is= IsEmpty
0 INVERT 1 AND   1 Is= IsEmpty
1 INVERT 1 AND   0 Is= IsEmpty
0S      0S AND  0S Is= IsEmpty
0S      1S AND  0S Is= IsEmpty
1S      0S AND  0S Is= IsEmpty
1S      1S AND  1S Is= IsEmpty

\ Header "Or", 0 ; ( a b -- a|b )
 0S 0S OR  0S Is= IsEmpty
 0S 1S OR  1S Is= IsEmpty
 1S 0S OR  1S Is= IsEmpty
 1S 1S OR  1S Is= IsEmpty

\ Header "Xor", 0 ; ( a b -- a^b )
 0S 0S XOR  0S Is= IsEmpty
 0S 1S XOR  1S Is= IsEmpty
 1S 0S XOR  1S Is= IsEmpty
 1S 1S XOR  0S Is= IsEmpty

\ Header "Invert", 0 ; ( a -- ~a )
 0S INVERT  1S Is= IsEmpty
 1S INVERT  0S Is= IsEmpty

Variable V1  0 ,  \ 2Variable
\ Header "@",0 ; ( addr -- val ) fetch cell
\ Header "!", 0 ; ( val addr -- ) store cell
1234 V1 !  IsEmpty
V1 @ 1234 Is= IsEmpty

\ Header "C@", 0 ; ( addr -- byte ) fetch byte
\ Header "C!", 0 ; ( byte addr -- ) store byte
56 V1 C! IsEmpty
V1 C@ 56 Is= IsEmpty

\ Header "2@", 0 ; ( addr -- d ) fetch double cell
\ Header "2!", 0 ; ( d addr -- ) store double cell
2345 6789 V1 2! IsEmpty
V1 2@ 2345 6789 Is2= IsEmpty 

Create SBuf  12 C, 34 C, 56 C,
Create FBuf  0 , 0 ,
\ Header "Fill", 0 ; ( caddr u byte -- ) fill u bytes starting at addr with byte
FBUF 0 20 FILL IsEmpty
  FBuf C@ 0 Is=  FBuf 1+ C@ 0 Is=  FBuf 2+ C@ 0 Is=
FBUF 1 20 FILL IsEmpty
  FBuf C@ 20 Is=  FBuf 1+ C@ 00 Is=  FBuf 2+ C@ 00 Is=
FBUF 3 20 FILL IsEmpty
  FBuf C@ 20 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=

\ Header "Move", 0 ; ( src dst u -- ) copy u bytes from src to dst
FBUF FBUF 3 CMOVE IsEmpty
  FBuf C@ 20 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=
SBUF FBUF 0 CMOVE IsEmpty
  FBuf C@ 20 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=
SBUF FBUF 1 CMOVE IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=
SBUF FBUF 3 CMOVE IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 34 Is=  FBuf 2+ C@ 56 Is=
FBUF FBUF 1+ 2  CMOVE IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 12 Is=  FBuf 2+ C@ 34 Is=
FBUF 1+ FBUF 2 CMOVE IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 34 Is=  FBuf 2+ C@ 34 Is=

\ Header "Key", 0 ; ( -- char ) receive character (blocking)
\ Header "Key?", 0 ; ( -- flag ) non-blocking check for available input
\ Header "Emit", 0 ; ( char -- ) transmit character
\ Header "CR",0 ; ( -- ) emit new line
\ Header "Space",0 ; ( -- ) emit a space
\ Header "Spaces",0 ; ( n -- ) emit n spaces
\ Header "Type",0 ; ( addr u -- ) transmit u characters from addr
\ Header "Accept",0 ; ( bufaddr buflen -- actualLen ) read a line from console into buffer
\ Header ".Hex", 0 ; ( n -- ) Print TOS as hex
\ Header "C.Hex",0 ; ( n -- ) Print TOS as 2-digit hex
\ Header "U.",0 ; ( u -- )  print as unsigned number
\ Header ".", 0 ; ( n -- ) print signed number

\ Header "Execute",0 ; ( xt -- ) execute word by execution token
1234 ' Dup Execute 1234 Is= 1234 Is= IsEmpty

\ Header "If",F_Immediate ; ( -- patch_addr )  Compile an If
\ Header "Else",F_Immediate ; ( patch_addr -- patch2_addr ) Compile "Else"
\ Header "Then",F_Immediate ; ( patch_addr -- )  Compile "Then"
: IfTest If 1111 Else 2222 Then ;
1 IfTest 1111 Is= IsEmpty
0 IfTest 2222 Is= IsEmpty

\ Header "Begin",F_Immediate ; ( -- rev_addr )  Compile "Begin"
\ Header "Again",F_Immediate ; ( rev_addr -- )  Compile "Again"
: AgainTest
  1 Begin  1+  Dup .  Swap Over + Swap  Over 5 > If Drop Exit Then  Again ;
0 AgainTest 9 Is= IsEmpty

\ Header "Until",F_Immediate ; ( rev_addr -- )  Compile "Until"
: UntilTest
  1 Begin  1+  Dup .  Swap Over + Swap  Over 5 > Until Drop ;
0 UntilTest 9 Is= IsEmpty

\ Header "While",F_Immediate ; ( rev_addr -- rev_addr fwd_addr )  Compile "While"
\ Header "Repeat",F_Immediate ; ( rev_addr fwd_addr -- )  Compile "Repeat"
: WhileTest
  1 Begin  1+  Dup 6 < While  Swap Over + Swap  Repeat  Drop ;
0 WhileTest 9 Is= IsEmpty

\ Header "Do",F_Immediate ; ( -- back_addr )  Compile a DO
\ Header "Loop",F_Immediate ; ( back-addr -- )  Compile LOOP
: LoopTest
  6 1 Do  I +  Loop ;
20 LoopTest 35 Is= IsEmpty

\ Header "+Loop",F_Immediate ; ( back-addr -- )  Compile +Loop
: +LoopTest1  6 1 do  I +  2 +Loop ;
20 +LoopTest1 29 Is= IsEmpty
: +LoopTest2  0 5 Do  I +  -2 +Loop ;
20 +LoopTest1 29 Is= IsEmpty

\ Header "Unloop",F_Immediate ; ( -- ) (R: limit index -- ) discard DO loop parameters
: UnloopTest ( n -- n' )
  5 1 Do  I 3 > If  Unloop Exit Then  I + Loop ;
10 UnloopTest 16 Is= IsEmpty

\ Header "I",0 ; ( -- n ) (R: limit index -- limit index) copy loop index
\ Header "J",0 ; ( -- n ) ( R: 2limit 2index 1limit 1index ) copy 2nd loop index
: IJTest ( n -- n' )
  5 1 Do  3 0 Do  I J * +  Loop  Loop ;
20 IJTest 50 Is= IsEmpty

\ Header "Here",0 ; ( -- addr ) current dictionary pointer

\ Header "Allot",0 ; ( n -- ) advance dictionary pointer by n bytes
Here  5 Allot  Here 5 + Is= IsEmpty

\ Header ",",0 ; ( val -- ) compile cell into dictionary
Here  4321 ,  Here - -2 Is= IsEmpty
Here 2 - @ 4321 Is= IsEmpty

\ Header "C,",0 ; ( byte -- ) compile byte into dictionary
Here 56 C, Here - -1 Is= IsEmpty
Here 1- C@ 56 Is= IsEmpty

\ Header "Compile,",0 ; ( xt -- )  Compile a jsr abs

\ Header "Jmp,",0 ; ( xt -- )  compile a jmp abs

\ Header "Exit",F_Immediate ; ( -- ) compile return from current colon definition

\ Header "Lda#,",0 ; ( n -- )  compile lda #

\ Header "Literal",F_Immediate ; ( n -- )  Compile inline constant
: LitTest  6789 ;
LitTest 6789 Is= IsEmpty

\ Header "Header,",0 ; ( addr len -- )  Compile a word header
\ Header "Name>String",0 ; ( nt -- c-addr u )  Given a name token, return name as a string
' Dup Name>String Type IsEmpty

\ Header "Words",0 ; ( -- )  Walk dictionary and print names
Words IsEmpty

\ Header "Search-WordList",0 ; ( caddr u wid -- 0 | xt 1 | xt -1)  "Search for a word in a wordlist"
\ Header "'",0 ; ( "name" -- xt )  find a word
\ Header "Constant",0 ; ( "name" n -- )  Define a constant word
\ Header "Variable",0 ; ( "name" -- )  Define a variable word
\ Header "Create",0 ; ( "name" -- )  Create a word that pushes the addr of it's parameter field
\ Header "Does>", F_Immediate ;
: abc Create , Does> @ ;  IsEmpty
4321 abc def  IsEmpty
def 4321 Is= IsEmpty

\ Header '."',F_Immediate ; ( string" -- )  Type a string literal
." Testing1"
: ."Test ." Testing2" ;
."Test

\ Header 'S"',F_Immediate ; ( -- caddr len )  create a string literal
S" test3" Dup 5 Is=  Type  IsEmpty
: S"Test  S" test3" ;
S"Test Dup 5 Is=  Type IsEmpty

\ Header "MustBeCompiling",0 ; ( -- ) make sure we're compiling
\ Header 'Abort"', F_Immediate ; If f is true, print string & abort
\ Header "Latest",0 ; ( -- addr ) address of LATEST variable in user area
\ Header "Base",0 ; ( -- addr ) address of BASE variable
Base @ 10 Is= IsEmpty

\ Header "State",0 ; ( -- addr ) address of STATE variable
State @ 0 Is= IsEmpty

\ Header ">In",0 ; ( -- addr ) address of >IN variable
>In @ 6 Is= IsEmpty

\ Header "Source",0 ; ( -- addr len ) current input source
Source Type IsEmpty

\ Header "Decimal",0 ; ( -- )  set base to 10
4 Base !  Decimal  Base @ Hex 0a Is= IsEmpty

\ Header "Hex",0 ; ( -- )  set base to 16
4 Base !  Hex  Base @ 0a Is= IsEmpty

Create CntBuf  3 C,
\ Header "Count",0 ; ( addr -- addr+1 len ) counted string to addr/len
CntBuf Count  3 Is=  Here Is= IsEmpty

\ Header "Parse",0 ; ( "name" c -- addr u )  "Parse input with delimiter character"
\ Header "Parse-Name",0 ; (  (spaces)name(space)  -- c-addr u )  Skip leading spaces. Parse space delimited name.
\ Header "Number",0 ; ( adr len -- 0 ) or ( adr len -- n -1 ) or ( adr len -- d -2 ) or ( adr len -- fp -3 )

\ Header "Bye",0 ; ( -- ) halt the system
\ Header "Abort",0 ; ( -- ) reset stacks and go to QUIT
\ Header "Quit", 0 ; ( -- ) outer interpreter loop
\ Header "Interpret",0 ; ( -- ) parse and execute/compile words from input

\ Header "[",F_Immediate ; ( -- )  switch State to interpret
\ Header "]",0 ; ( -- )  switch State to compile
: []Test  3 [ 7 ] Literal * ;
[]Test 21 Is= IsEmpty

\ Header ":", 0 ; ( "name" -- )  start compiling a new colon word
\ Header ";",F_Immediate ;  finish compiling a new colon word

\ Header "Dump",0 ; ( caddr len -- )  Dump memory in hex
400 50 Dump IsEmpty

\ Header ".S",0 ; ( -- ) print stack contents non-destructively
1 2 3 .s  PSP-Reset

