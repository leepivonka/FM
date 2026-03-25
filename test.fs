\ FM FORTH tests
\ Some test adapted from https://forth-standard.org

PSP-Reset   \ clear parm stack
: IsEmpty ( -- )  Depth Abort" Stack not empty"  ;  IsEmpty
: Is= ( a b -- )  <> Abort" Mismatch!" ;  IsEmpty
: Is2= ( da db -- )  Rot <> >R <> R> Or Abort" 2Mismatch!" ;  IsEmpty

Hex  IsEmpty   \ numbers in hex

   0 Constant 0S	  IsEmpty  0S     0 Is= IsEmpty 
ffff Constant 1S	  IsEmpty  1S  ffff Is= IsEmpty
8000 Constant MSB
7fff Constant Max-Int 
8000 Constant Min-Int 
   0 Constant Min-UInt 
7fff Constant Mid-UInt 
8000 Constant Mid-UInt+1  IsEmpty  Mid-UInt+1 8000 Is= IsEmpty
ffff Constant Max-UInt 

\ ---- Parm Stack ----------------------------------------------

\ Header "PSP-Reset",0 ; ( ... -- )  clear parameter stack
1111 2222 PSP-Reset IsEmpty

\ Header "Dup",0 ; ( a -- a a )
1234 Dup 1234 Is= 1234 Is= IsEmpty

\ Header "Drop", 0 ; ( a -- )
2345 Drop IsEmpty

\ Header "Swap", 0 ; ( a b -- b a )
3456 4567 Swap 3456 Is= 4567 Is= IsEmpty

\ Header "Over", 0 ; ( a b -- a b a )
5678 6789 Over 5678 Is= 6789 Is= 5678 Is= IsEmpty

\ Header "Rot", 0 ; ( a b c -- b c a )
1234 2345 3456 Rot 1234 Is= 3456 Is= 2345 Is= IsEmpty

\ Header "Nip",0 ; ( a b -- b )
4321 5432 Nip 5432 Is= IsEmpty

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
110 220 330 Depth 3 Is= 330 Is= 220 Is= 110 Is= IsEmpty

\ Header "Pick",0 ; ( xu...x1 x0 u -- xu...x1 x0 xu )
1111 2222 3333 1 Pick 2222 Is= 3333 Is= 2222 Is= 1111 Is= IsEmpty

\ -------------- Return stack ----------------------

\ Header ">R", 0 ; ( a -- ) (R: -- a )  push to return stack
\ Header "R@", 0 ; ( -- a ) (R: a -- a )  get a copy of top of return stack
\ Header "R>", 0 ; ( -- a ) (R: a -- )  pop from return stack
: TestR1 ( -- )
  1221 >R 2332 >R IsEmpty
  R@ 2332 Is= IsEmpty
  R> 2332 Is= R> 1221 Is= IsEmpty
  ;  IsEmpty
TestR1 IsEmpty

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
      0       1 Max        1 Is= IsEmpty
      1       2 Max        2 Is= IsEmpty
     -1       0 Max        0 Is= IsEmpty
     -1       1 Max        1 Is= IsEmpty
Min-Int       0 Max        0 Is= IsEmpty
Min-Int Max-Int Max  Max-Int Is= IsEmpty
      0 Max-Int Max  Max-Int Is= IsEmpty
      0       0 Max        0 Is= IsEmpty
      1       1 Max        1 Is= IsEmpty
      1       0 Max        1 Is= IsEmpty
      2       1 Max        2 Is= IsEmpty
      0      -1 Max        0 Is= IsEmpty
      1      -1 Max        1 Is= IsEmpty
      0 Min-Int Max        0 Is= IsEmpty
Max-Int Min-Int Max  Max-Int Is= IsEmpty
Max-Int       0 Max  Max-Int Is= IsEmpty

\ Header "Min",0 ; ( a b -- min )  signed
   0032    1032 Min     0032 Is= IsEmpty
   0032    fedc Min     fedc Is= IsEmpty
   1234    8032 Min     8032 Is= IsEmpty
      0       1 Min        0 Is= IsEmpty
      1       2 Min        1 Is= IsEmpty
     -1       0 Min       -1 Is= IsEmpty
     -1       1 Min       -1 Is= IsEmpty
Min-Int       0 Min  Min-Int Is= IsEmpty
Min-Int Max-Int Min  Min-Int Is= IsEmpty
      0 Max-Int Min        0 Is= IsEmpty
      0       0 Min        0 Is= IsEmpty
      1       1 Min        1 Is= IsEmpty
      1       0 Min        0 Is= IsEmpty
      2       1 Min        1 Is= IsEmpty
      0      -1 Min       -1 Is= IsEmpty
      1      -1 Min       -1 Is= IsEmpty
      0 Min-Int Min  Min-Int Is= IsEmpty
Max-Int Min-Int Min  Min-Int Is= IsEmpty
Max-Int       0 Min        0 Is= IsEmpty

\ Header "2/",0 ; ( n -- n/2 ) signed shift right
0537 2/ 029B Is= IsEmpty
fedc 2/ ff6e Is= IsEmpty

\ Header "U2/",0 ; ( u -- u/2 ) unsigned shift right
0537 U2/ 029B Is= IsEmpty
fedc U2/ 7f6e Is= IsEmpty

\ Header "LShift",0 ; ( a u -- a<<u ) logical shift left
1234 4 LShift 2340 Is= IsEmpty

\ Header "RShift",0 ; ( a u -- a>>u ) logical shift right
1234 4 RShift 0123 Is= IsEmpty

\ Header "S>D",0 ; ( n -- d )  Convert the signed number n to the double-cell number d
1234 S>D 0000 Is= 1234 Is= IsEmpty
fedc S>D ffff Is= fedc Is= IsEmpty

\ Header "UM*",0 ; ( u1 u2 -- ud ) unsigned 16x16 -> 32-bit result
      1025        255 UM* 0025 Is= A649 Is= IsEmpty
         0          0 UM*  0 Is= 0 Is= IsEmpty
         0          1 UM*  0 Is= 0 Is= IsEmpty
         1          0 UM*  0 Is= 0 Is= IsEmpty
         1          2 UM*  0 Is= 2 Is= IsEmpty
         2          1 UM*  0 Is= 2 Is= IsEmpty
         3          3 UM*  0 Is= 9 Is= IsEmpty
Mid-UInt+1 1 RSHIFT 2 UM*  0 Is=  Mid-UInt+1 Is= IsEmpty
Mid-UInt+1          2 UM*  1 Is=           0 Is= IsEmpty
Mid-UInt+1          4 UM*  2 Is=           0 Is= IsEmpty
        1S          2 UM*  1 Is=  1S 1 LShift Is= IsEmpty
  Max-UInt   Max-UInt UM*  1 Invert Is= 1 Is= IsEmpty

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
      0 Min-Int M*        0 S>D Is2= IsEmpty
      1 Min-Int M*  Min-Int  S>D Is2= IsEmpty
      2 Min-Int M*        0 1S  Is2= IsEmpty
      0 Max-Int M*        0 S>D Is2= IsEmpty
      1 Max-Int M*  Max-Int  S>D Is2= IsEmpty
      2 Max-Int M*  Max-Int      1 LSHIFT 0 Is2= IsEmpty
Min-Int Min-Int M*        0 MSB 1 RSHIFT   Is2= IsEmpty
Max-Int Min-Int M*      MSB MSB 2/         Is2= IsEmpty
Max-Int Max-Int M*        1 MSB 2/ INVERT  Is2= IsEmpty

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
Mid-UInt+1 1 RSHIFT 2 *                Mid-UInt+1 Is= IsEmpty
Mid-UInt+1 2 RSHIFT 4 *                Mid-UInt+1 Is= IsEmpty
Mid-UInt+1 1 RSHIFT Mid-UInt+1 OR 2 *  Mid-UInt+1 Is= IsEmpty

\ Header "UM/Mod", 0 ; ( ud u -- ur uq ) unsigned 32/16 -> 16 remainder, 16 quotient
27c0 0009 000a UM/Mod EA60 Is= 0000 Is= IsEmpty

\ Header "SM/Rem ",0 ; ( d1 n1 -- n_remainder n_quotient )  Symmetric signed division
       0 S>D              1 SM/Rem         0 Is=  0 Is= IsEmpty
       1 S>D              1 SM/Rem         1 Is=  0 Is= IsEmpty
       2 S>D              1 SM/Rem         2 Is=  0 Is= IsEmpty
      -1 S>D              1 SM/Rem        -1 Is=  0 Is= IsEmpty
      -2 S>D              1 SM/Rem        -2 Is=  0 Is= IsEmpty
       0 S>D             -1 SM/Rem         0 Is=  0 Is= IsEmpty
       1 S>D             -1 SM/Rem        -1 Is=  0 Is= IsEmpty
       2 S>D             -1 SM/Rem        -2 Is=  0 Is= IsEmpty
      -1 S>D             -1 SM/Rem         1 Is=  0 Is= IsEmpty
      -2 S>D             -1 SM/Rem         2 Is=  0 Is= IsEmpty
       2 S>D              2 SM/Rem         1 Is=  0 Is= IsEmpty
      -1 S>D             -1 SM/Rem         1 Is=  0 Is= IsEmpty
      -2 S>D             -2 SM/Rem         1 Is=  0 Is= IsEmpty
       7 S>D              3 SM/Rem         2 Is=  1 Is= isEmpty
       7 S>D             -3 SM/Rem        -2 Is=  1 Is= IsEmpty
      -7 S>D              3 SM/Rem        -2 Is= -1 Is= IsEmpty
      -7 S>D             -3 SM/Rem         2 Is= -1 Is= IsEmpty
 Max-Int  S>D              1 SM/Rem   Max-Int  Is=  0 Is= IsEmpty
 Min-Int  S>D              1 SM/Rem   Min-Int  Is=  0 Is= IsEmpty
 Max-Int  S>D        Max-Int  SM/Rem         1 Is=  0 Is= IsEmpty
 Min-Int  S>D        Min-Int  SM/Rem         1 Is=  0 Is= IsEmpty
      1S 1                4 SM/Rem   Max-Int  Is=  3 Is= IsEmpty
       2 Min-Int  M*       2 SM/Rem   Min-Int  Is=  0 Is= IsEmpty
       2 Min-Int  M* Min-Int  SM/Rem         2 Is=  0 Is= IsEmpty
       2 Max-Int  M*       2 SM/Rem   Max-Int  Is=  0 Is= IsEmpty
       2 Max-Int  M* Max-Int  SM/Rem         2 Is=  0 Is= IsEmpty
 Min-Int  Min-Int  M* Min-Int  SM/Rem   Min-Int  Is=  0 Is= IsEmpty
 Min-Int  Max-Int  M* Min-Int  SM/Rem   Max-Int  Is=  0 Is= IsEmpty
 Min-Int  Max-Int  M* Max-Int  SM/Rem   Min-Int  Is=  0 Is= IsEmpty
 Max-Int  Max-Int  M* Max-Int  SM/Rem   Max-Int  Is=  0 Is= IsEmpty

\ Header "/MOD", 0 ; ( n1 n2 -- rem quot ) signed division
7fff 0a /Mod 0CCC Is= 0007 Is= IsEmpty

\ Header "/", 0 ; ( n1 n2 -- quot ) signed division
-7655 1234 / -6 Is= IsEmpty

\ ---------- Logic ---------------

\ Header "True", 0 ; ( -- true )
True ffff Is= IsEmpty

\ Header "False", 0 ; ( -- false )
False 0 Is= IsEmpty

\ Header "0=",0 ; ( n -- flag )
   1 0= False Is= IsEmpty
   0 0= True  Is= IsEmpty
8000 0= False Is= IsEmpty

\ Header "0<",0 ; ( n -- flag )
   1 0< False Is= IsEmpty
   0 0< False Is= IsEmpty
  -1 0< True  Is= IsEmpty
8000 0< True  Is= IsEmpty
7fff 0< False Is= IsEmpty

\ Header "0>",0 ; ( n -- flag )
   1 0> True  Is= IsEmpty
   0 0> False Is= IsEmpty
  -1 0> False Is= IsEmpty
8000 0> False Is= IsEmpty
7fff 0> True  Is= IsEmpty

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
Min-Int       0 < True  Is= IsEmpty
Min-Int Max-Int < True  Is= IsEmpty
      0 Max-Int < True  Is= IsEmpty
      0       0 < False Is= IsEmpty
      1       1 < False Is= IsEmpty
      1       0 < False Is= IsEmpty
      2       1 < False Is= IsEmpty
      0      -1 < False Is= IsEmpty
      1      -1 < False Is= IsEmpty
      0 Min-Int < False Is= IsEmpty
Max-Int Min-Int < False Is= IsEmpty
Max-Int       0 < False Is= IsEmpty

\ Header ">",0 ; ( a b -- flag ) signed
      0       1 > False Is= IsEmpty
      1       2 > False Is= IsEmpty
     -1       0 > False Is= IsEmpty
     -1       1 > False Is= IsEmpty
Min-Int       0 > False Is= IsEmpty
Min-Int Max-Int > False Is= IsEmpty
      0 Max-Int > False Is= IsEmpty
      0       0 > False Is= IsEmpty
      1       1 > False Is= IsEmpty
      1       0 > True  Is= IsEmpty
      2       1 > True  Is= IsEmpty
      0      -1 > True  Is= IsEmpty
      1      -1 > True  Is= IsEmpty
      0 Min-Int > True  Is= IsEmpty
Max-Int Min-Int > True  Is= IsEmpty
Max-Int       0 > True  Is= IsEmpty

\ Header "U<",0 ; ( u1 u2 -- flag ) unsigned less than
       0        1 U< True  Is= IsEmpty
       1        2 U< True  Is= IsEmpty
       0 Mid-UInt  U< True  Is= IsEmpty
       0 Max-UInt  U< True  Is= IsEmpty
Mid-UInt  Max-UInt  U< True  Is= IsEmpty
       0        0 U< False Is= IsEmpty
       1        1 U< False Is= IsEmpty
       1        0 U< False Is= IsEmpty
       2        1 U< False Is= IsEmpty
Mid-UInt         0 U< False Is= IsEmpty
Max-UInt         0 U< False Is= IsEmpty
Max-UInt  Mid-UInt  U< False Is= IsEmpty

\ Header "U>",0 ; ( u1 u2 -- flag ) unsigned greater than
       0        1 U> False Is= IsEmpty
       1        2 U> False Is= IsEmpty
       0 Mid-UInt  U> False Is= IsEmpty
       0 Max-UInt  U> False Is= IsEmpty
Mid-UInt  Max-UInt  U> False Is= IsEmpty
       0        0 U> False Is= IsEmpty
       1        1 U> False Is= IsEmpty
       1        0 U> True  Is= IsEmpty
       2        1 U> True  Is= IsEmpty
Mid-UInt         0 U> True  Is= IsEmpty
Max-UInt         0 U> True  Is= IsEmpty
Max-UInt  Mid-UInt  U> True  Is= IsEmpty

\ Header "And", 0 ; ( a b -- a&b )
0        0 AND   0  Is= IsEmpty
0        1 AND   0  Is= IsEmpty
1        0 AND   0  Is= IsEmpty
1        1 AND   1  Is= IsEmpty
0 INVERT 1 AND   1  Is= IsEmpty
1 INVERT 1 AND   0  Is= IsEmpty
0S      0S AND  0S  Is= IsEmpty
0S      1S AND  0S  Is= IsEmpty
1S      0S AND  0S  Is= IsEmpty
1S      1S AND  1S  Is= IsEmpty
1234  5678 And 1230 Is= IsEmpty

\ Header "Or", 0 ; ( a b -- a|b )
 0S   0S  Or  0S  Is= IsEmpty
 0S   1S  Or  1S  Is= IsEmpty
 1S   0S  Or  1S  Is= IsEmpty
 1S   1S  Or  1S  Is= IsEmpty
1234 5678 Or 567c Is= IsEmpty

\ Header "Xor", 0 ; ( a b -- a^b )
 0S   0S  Xor  0S  Is= IsEmpty
 0S   1S  Xor  1S  Is= IsEmpty
 1S   0S  Xor  1S  Is= IsEmpty
 1S   1S  Xor  0S  Is= IsEmpty
1234 5678 Xor 444c Is= IsEmpty

\ Header "Invert", 0 ; ( a -- ~a )
 0S  Invert  1S  Is= IsEmpty
 1S  Invert  0S  Is= IsEmpty
1234 Invert edcb Is= IsEmpty

\ Header "Variable",0 ; ( "name" -- )  Define a variable word
Variable V1  0 ,  \ 2Variable
V1 4 + Here Is= IsEmpty

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
V1 @ 6789 Is=  V1 2+ @ 2345 Is=  IsEmpty
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

\ Header "CMove", 0 ; ( src dst u -- ) copy u bytes from src to dst
FBUF FBUF 3 CMove IsEmpty
  FBuf C@ 20 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=
SBUF FBUF 0 CMove IsEmpty
  FBuf C@ 20 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=
SBUF FBUF 1 CMove IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=
SBUF FBUF 3 CMove IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 34 Is=  FBuf 2+ C@ 56 Is=
FBUF FBUF 1+ 2 CMove IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 12 Is=  FBuf 2+ C@ 12 Is=
SBUF FBUF 3 CMove IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 34 Is=  FBuf 2+ C@ 56 Is=
FBUF 1+ FBUF 2 CMove IsEmpty
  FBuf C@ 34 Is=  FBuf 1+ C@ 56 Is=  FBuf 2+ C@ 56 Is=

\ Header "Key", 0 ; ( -- char ) receive character (blocking)
\ Header "Key?", 0 ; ( -- flag ) non-blocking check for available input
\ Header "Emit", 0 ; ( char -- ) transmit character
45 Emit IsEmpty

\ Header "CR",0 ; ( -- ) emit new line
CR IsEmpty

\ Header "Space",0 ; ( -- ) emit a space
Space IsEmpty

\ Header "Spaces",0 ; ( n -- ) emit n spaces
4 Spaces IsEmpty

\ Header "Type",0 ; ( addr u -- ) transmit u characters from addr
S" abxy" Dup 4 Is=  Type IsEmpty

\ Header "Accept",0 ; ( bufaddr buflen -- actualLen ) read a line from console into buffer
\ Header ".Hex", 0 ; ( n -- ) Print TOS as hex
1234 .Hex IsEmpty

\ Header "C.Hex",0 ; ( n -- ) Print TOS as 2-digit hex
56 C.Hex IsEmpty

\ Header "U.",0 ; ( u -- )  print as unsigned number
ffec U. IsEmpty

\ Header ".", 0 ; ( n -- ) print signed number
ffec . IsEmpty

\ Header "Execute",0 ; ( xt -- ) execute word by execution token
1234 ' Dup Execute 1234 Is= 1234 Is= IsEmpty

\ Header "If",F_Immediate ; ( -- patch_addr )  Compile an If
\ Header "Else",F_Immediate ; ( patch_addr -- patch2_addr ) Compile "Else"
\ Header "Then",F_Immediate ; ( patch_addr -- )  Compile "Then"
: IfTest If 1111 Else 2222 Then ;
   1 IfTest 1111 Is= IsEmpty
   0 IfTest 2222 Is= IsEmpty
edcb IfTest 1111 Is= IsEmpty
: GI1 If 123 Then ; IsEmpty
 0 GI1  IsEmpty
 1 GI1  123 Is= IsEmpty
-1 GI1  123 Is= IsEmpty
: GI2 If 123 Else 234 Then ; IsEmpty
 0 GI2  234 Is= IsEmpty
 1 GI2  123 Is= IsEmpty
-1 GI2  123 Is= IsEmpty

\ Header "Begin",F_Immediate ; ( -- rev_addr )  Compile "Begin"
\ Header "Again",F_Immediate ; ( rev_addr -- )  Compile "Again"
: AgainTest
  1 Begin  1+  Dup .  Swap Over + Swap  Over 5 > If Drop Exit Then  Again ;
0 AgainTest 9 Is= IsEmpty

\ Header "Until",F_Immediate ; ( rev_addr -- )  Compile "Until"
: UntilTest
  1 Begin  1+  Dup .  Swap Over + Swap  Over 5 > Until Drop ;
0 UntilTest 9 Is= IsEmpty
: GI4 Begin Dup 1+ Dup 5 > Until ; IsEmpty
3 GI4
  6 Is= 5 Is= 4 Is= 3 Is= IsEmpty
5 GI4
  6 Is= 5 Is= IsEmpty
6 GI4
  7 Is= 6 Is= IsEmpty

\ Header "While",F_Immediate ; ( rev_addr -- rev_addr fwd_addr )  Compile "While"
\ Header "Repeat",F_Immediate ; ( rev_addr fwd_addr -- )  Compile "Repeat"
: WhileTest
  1 Begin  1+  Dup 6 < While  Swap Over + Swap  Repeat  Drop ;
0 WhileTest 0e Is= IsEmpty
: GI3 Begin Dup 5 < While Dup 1+ Repeat ; IsEmpty
0 GI3
  5 Is= 4 Is= 3 Is= 2 Is= 1 Is= 0 Is= IsEmpty
4 GI3
  5 Is= 4 Is= IsEmpty
5 GI3
  5 Is= IsEmpty
6 GI3
  6 Is= IsEmpty


\ Header "Do",F_Immediate ; ( -- back_addr )  Compile a DO
\ Header "Loop",F_Immediate ; ( back-addr -- )  Compile LOOP
: LoopTest
  6 1 Do  I +  Loop ;
20 LoopTest 2f Is= IsEmpty
: GD1 Do I Loop ; IsEmpty
         4        1 GD1  3 Is= 2 Is= 1 Is= IsEmpty
         2       -1 GD1  1 Is= 0 Is= -1 Is= IsEmpty
Mid-UInt+1 Mid-UInt GD1  Mid-UInt Is= IsEmpty

\ Header "+Loop",F_Immediate ; ( back-addr -- )  Compile +Loop
: +LoopTest1  6 1 do  I +  2 +Loop ;
20 +LoopTest1 29 Is= IsEmpty
: +LoopTest2  0 5 Do  I +  -2 +Loop ;
20 +LoopTest1 29 Is= IsEmpty

: +!  Dup >R @ + R> ! ;
VARIABLE gditerations
VARIABLE gdincrement
: gd7 ( limit start increment -- )
   gdincrement !
   0 gditerations !
   DO
     1 gditerations +!
     I dup .
     gditerations @ 6 = IF ( LEAVE ) Unloop gditerations @ Exit THEN
     gdincrement @
   +LOOP gditerations @
  ;  IsEmpty
Decimal
   4  4  -1 gd7   1 Is=  4 Is= IsEmpty
   1  4  -1 gd7   4 Is= 1 Is= 2 Is= 3 Is= 4 Is= IsEmpty
   4  1  -1 gd7   6 Is= -4 Is= -3 Is= -2 Is= -1 Is= 0 Is= 1 Is= IsEmpty
   4  1   0 gd7   6 Is= 1 Is= 1 Is= 1 Is= 1 Is= 1 Is= 1 Is= IsEmpty
   0  0   0 gd7   6 Is= 0 Is= 0 Is= 0 Is= 0 Is= 0 Is= 0 Is= IsEmpty
   1  4   0 gd7   6 Is= 4 Is= 4 Is= 4 Is= 4 Is= 4 Is= 4 Is= IsEmpty
   1  4   1 gd7   6 Is= 9 Is= 8 Is= 7 Is= 6 Is= 5 Is= 4 Is= IsEmpty
   4  1   1 gd7   3 Is= 3 Is= 2 Is= 1 Is= IsEmpty
   4  4   1 gd7   6 Is= 9 Is= 8 Is= 7 Is= 6 Is= 5 Is= 4 Is= IsEmpty
   2 -1  -1 gd7   6 Is= -6 Is= -5 Is= -4 Is= -3 Is= -2 Is= -1 Is= IsEmpty
  -1  2  -1 gd7   4 Is= -1 Is= 0 Is= 1 Is= 2 Is= IsEmpty
   2 -1   0 gd7   6 Is= -1 Is= -1 Is= -1 Is= -1 Is= -1 Is= -1 Is= IsEmpty
  -1  2   0 gd7   6 Is= 2 Is= 2 Is= 2 Is= 2 Is= 2 Is= 2 Is= IsEmpty
  -1  2   1 gd7   6 Is= 7 Is= 6 Is= 5 Is= 4 Is= 3 Is= 2 Is= IsEmpty
   2 -1   1 gd7   3 Is= 1 Is= 0 Is= -1 Is= IsEmpty
 -20 30 -10 gd7   6 Is= -20 Is= -10 Is= 0 Is= 10 Is= 20 Is= 30 Is= IsEmpty
 -20 31 -10 gd7   6 Is= -19 Is= -9 Is= 1 Is= 11 Is= 21 Is= 31 Is= IsEmpty
 -20 29 -10 gd7   5 Is= -11 Is= -1 Is= 9 Is= 19 Is= 29 Is= IsEmpty 

\ With large and small increments
MAX-UINT 8 RSHIFT 1+ CONSTANT ustep
ustep NEGATE CONSTANT -ustep
MAX-INT 7 RSHIFT 1+ CONSTANT step
step NEGATE CONSTANT -step
VARIABLE bump
: gd8 bump ! DO 1+ bump @ +LOOP ; IsEmpty
 0 MAX-UINT 0 ustep gd8  256 Is= IsEmpty
 0 0 MAX-UINT -ustep gd8  256 Is= IsEmpty
 0 MAX-INT MIN-INT step gd8  256 Is= IsEmpty
 0 MIN-INT MAX-INT -step gd8  256 Is= IsEmpty
Hex

\ Header "Unloop",F_Immediate ; ( -- ) (R: limit index -- ) discard DO loop parameters
: UnloopTest ( n -- n' )
  5 1 Do  I 3 > If  Unloop Exit Then  I + Loop ;
10 UnloopTest 16 Is= IsEmpty
: GD6 ( PAT: {0 0},{0 0}{1 0}{1 1},{0 0}{1 0}{1 1}{2 0}{2 1}{2 2} )
   0 SWAP 0 DO
      I 1+ 0 DO
        I J + 3 = IF I UNLOOP I UNLOOP EXIT THEN 1+
      LOOP
   LOOP ; IsEmpty
1 GD6  1 Is= IsEmpty
2 GD6  3 Is= IsEmpty
3 GD6  2 Is= 1 Is= 4 Is= IsEmpty

\ Header "I",0 ; ( -- n ) (R: limit index -- limit index) copy loop index
\ Header "J",0 ; ( -- n ) ( R: 2limit 2index 1limit 1index ) copy 2nd loop index
: IJTest ( n -- n' )
  5 1 Do  3 0 Do  I J * Dup . +  Loop  Loop ;
Decimal
20 IJTest 50 Is= IsEmpty
Hex

\ Header "Here",0 ; ( -- addr ) current dictionary pointer

\ Header "Allot",0 ; ( n -- ) advance dictionary pointer by n bytes
Here 5 +  5 Allot  Here Is= IsEmpty

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
' Nip Name>String Type IsEmpty

\ Header "Constant",0 ; ( "name" n -- )  Define a constant word
abcd Constant K1  IsEmpty
K1 abcd Is= IsEmpty

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
4 Base !  Decimal  Base @ Hex     0a Is= IsEmpty

\ Header "Hex",0 ; ( -- )  set base to 16
4 Base !  Hex      Base @ Decimal 16 Is= IsEmpty
Hex

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
Decimal
: []Test  3 [ 7 ] Literal * ;
[]Test 21 Is= IsEmpty
Hex

\ Header ":", 0 ; ( "name" -- )  start compiling a new colon word
\ Header ";",F_Immediate ;  finish compiling a new colon word

\ Header "Dump",0 ; ( caddr len -- )  Dump memory in hex
400 50 Dump IsEmpty

\ Header ".S",0 ; ( -- ) print stack contents non-destructively
1 2 3 .s  PSP-Reset

\ Done!
