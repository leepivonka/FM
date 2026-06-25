\ FM FORTH tests
\ Some test adapted from https://forth-standard.org
\ https://forth-standard.org/standard/testsuite

Decimal

Code CC@ ( -- ud ) \ get simulator's CPU cycle counter
  $5a C,		\ phy	save IP
  $02 C, $f5 C,		\ cop $f5	YA= CPU simulator cycle counter
  $caca , $caca ,	\ dex : dex : dex : dex
  $94 c, PStack    C,	\ sty PStack+0,x
  $95 c, PStack 2+ C,	\ sta PStack+2,x
  $7a C,		\ ply	restore IP
  ;Code  SeeLatest
: Timer ( "name" -- )
  '  CC@ 2>R Execute CC@ 2R> D- -120 M+
  \ -120 is for STC, doesn't auto-calibrate yet.
  Base @ >R Decimal DU. R> Base ! ." cycles" ; SeeLatest
Timer Chars  \ test Timer calibration on a NOP word

1 2 3  PSP-Reset  \ clear parm stack
: IsEmpty ( -- )  Depth Abort" Stack not empty"  ;  IsEmpty  SeeLatest
: Is= ( a b -- )  <> Abort" Mismatch!" ;  IsEmpty SeeLatest
: Is2= ( da db -- )  Rot <> >R <> R> Or Abort" 2Mismatch!" ;  IsEmpty SeeLatest

Hex  IsEmpty   \ numbers in hex

       0 Constant 0S	  IsEmpty SeeLatest  0S     0 Is= IsEmpty
0 Invert Constant 1S	  IsEmpty SeeLatest  1S $ffff Is= IsEmpty
    8000 Constant MSB     IsEmpty SeeLatest  MSB $8000 Is= IsEmpty
       0 Constant Min-UInt IsEmpty SeeLatest  Min-UInt 0 Is= IsEmpty

0 Invert          Constant Max-UInt   SeeLatest IsEmpty Max-UInt $ffff Is= IsEmpty
0 Invert 1 RShift Constant Max-Int    SeeLatest IsEmpty Max-Int $7fff Is= IsEmpty
0 Invert 1 RShift Invert Constant Min-Int SeeLatest IsEmpty Min-Int $8000 Is= IsEmpty
0 Invert 1 RShift Constant Mid-UInt   SeeLatest IsEmpty Mid-UInt $7fff Is= IsEmpty
0 Invert 1 RShift Invert Constant Mid-UInt+1 SeeLatest IsEmpty Mid-UInt+1 $8000 Is= IsEmpty

0S Constant <FALSE>  SeeLatest IsEmpty  <FALSE> 0 Is= IsEmpty
1S Constant <TRUE>   SeeLatest IsEmpty  <TRUE> -1 Is= IsEmpty

Max-Int 2/ Constant Hi-Int  SeeLatest IsEmpty  Hi-Int $3fff Is= IsEmpty \ 001...1
Min-Int 2/ Constant Lo-Int  SeeLatest IsEmpty  Lo-Int $c000 Is= IsEmpty \ 110...1

 1S MAX-INT 2Constant Max-2Int  IsEmpty SeeLatest Max-2Int $7fffffff. Is2= IsEmpty \ 01...1
  0 MIN-INT 2Constant Min-2Int  IsEmpty SeeLatest Min-2Int $80000000. Is2= IsEmpty \ 10...0
MAX-2INT 2/ 2Constant Hi-2Int   IsEmpty SeeLatest  Hi-2Int $3fffffff. Is2= IsEmpty \ 001...1
MIN-2INT 2/ 2Constant Lo-2Int   IsEmpty SeeLatest  Lo-2Int $c0000000. Is2= IsEmpty \ 110...0

Decimal  IsEmpty
#1289  1289 Is= IsEmpty
#12346789.  12346789. Is2= IsEmpty
#-1289 -1289 Is= IsEmpty
#-12346789. -12346789. Is2= IsEmpty
$12eF 4847 Is= IsEmpty
$12aBcDeF.  313249263. Is2= IsEmpty
$-12eF -4847 Is= IsEmpty
$-12AbCdEf. -313249263. Is2= IsEmpty
%10010110  150 Is= IsEmpty
%10010110.  150. Is2= IsEmpty
%-10010110  -150 Is= IsEmpty
%-10010110. -150. Is2= IsEmpty
'z' 122 Is= IsEmpty

Hex

\ ---- Parm Stack ----------------------------------------------

\ Header "Nip",0 ; ( a b -- b )
4321 5432 Timer Nip 5432 Is= IsEmpty

\ Header "PSP-Reset",0 ; ( ... -- )  clear parameter stack
1111 2222 Timer PSP-Reset IsEmpty

\ Header "Dup",0 ; ( a -- a a )
1234 Timer Dup 1234 Is= 1234 Is= IsEmpty

\ Header "?Dup",0 ; ( x -- 0 | x x )
 -1 Timer ?DUP  -1 Is= -1 Is= IsEmpty
  0 Timer ?DUP   0 Is= IsEmpty
  1 Timer ?DUP   1 Is= 1 Is= IsEmpty

\ Header "Over", 0 ; ( a b -- a b a )
5678 6789 Timer Over 5678 Is= 6789 Is= 5678 Is= IsEmpty

\ Header "Drop", 0 ; ( a -- )
3456 2345 Timer Drop 3456 Is= IsEmpty

\ Header "Swap", 0 ; ( a b -- b a )
3456 4567 Timer Swap 3456 Is= 4567 Is= IsEmpty

\ Header "Rot", 0 ; ( a b c -- b c a )
1234 2345 3456 Timer Rot 1234 Is= 3456 Is= 2345 Is= IsEmpty

\ Header "Tuck",0 ; ( a b -- b a b )
1234 5678 Timer Tuck 5678 Is= 1234 Is= 5678 Is= IsEmpty

\ Header "2Drop", 0 ; ( a b -- )
3210 2109 1098 Timer 2Drop 3210 Is= IsEmpty

\ Header "2Rot",0 ; ( x1 x2 x3 x4 x5 x6 -- x3 x4 x5 x6 x1 x2 )
       1.       2. 3. Timer 2ROT        1. Is2= 3. Is2=       2. Is2= IsEmpty
 MAX-2INT MIN-2INT 1. Timer 2ROT  Max-2Int Is2= 1. Is2= Min-2Int Is2= IsEmpty

\ Header "2Dup", 0 ; ( a b -- a b a b )
1234 2345 3456 Timer 2Dup 2345 3456 Is2=  2345 3456 Is2= 1234 Is= IsEmpty

\ Header "2Over", 0 ; ( a b c d -- a b c d a b )
1234 2345 3456 4567 5678 Timer 2Over 2345 3456 Is2=  4567 5678 Is2= 2345 3456 Is2=  1234 Is= IsEmpty

\ Header "2Nip",0 ; ( da db -- db )
12345678. 23456789. Timer 2Nip  23456789. Is2= IsEmpty
1234 2345 3456 4567 Timer 2Nip  45673456. Is2= IsEmpty

\ Header "DMax",0 ; ( d1 d2 -- d3 )
       1.       2. Timer DMax   2.      Is2= IsEmpty
       1.       0. Timer DMax   1.      Is2= IsEmpty
       1.      -1. Timer DMax   1.      Is2= IsEmpty
       1.       1. Timer DMax   1.      Is2= IsEmpty
       0.       1. Timer DMax   1.      Is2= IsEmpty
       0.      -1. Timer DMax   0.      Is2= IsEmpty
      -1.       1. Timer DMax   1.      Is2= IsEmpty
      -1.      -2. Timer DMax  -1.      Is2= IsEmpty
 MAX-2INT  HI-2INT Timer DMax  MAX-2INT Is2= IsEmpty
 MAX-2INT MIN-2INT Timer DMax  MAX-2INT Is2= IsEmpty
 MIN-2INT MAX-2INT Timer DMax  MAX-2INT Is2= IsEmpty
 MIN-2INT  LO-2INT Timer DMax  LO-2INT  Is2= IsEmpty
 MAX-2INT       1. Timer DMax  MAX-2INT Is2= IsEmpty
 MAX-2INT      -1. Timer DMax  MAX-2INT Is2= IsEmpty
 MIN-2INT       1. Timer DMax   1.      Is2= IsEmpty
 MIN-2INT      -1. Timer DMax  -1.      Is2= IsEmpty

\ Header "DMin",0 ; ( d1 d2 -- d3 )
       1.       2. Timer DMin   1.      Is2= IsEmpty
       1.       0. Timer DMin   0.      Is2= IsEmpty
       1.      -1. Timer DMin  -1.      Is2= IsEmpty
       1.       1. Timer DMin   1.      Is2= IsEmpty
       0.       1. Timer DMin   0.      Is2= IsEmpty
       0.      -1. Timer DMin  -1.      Is2= IsEmpty
      -1.       1. Timer DMin  -1.      Is2= IsEmpty
      -1.      -2. Timer DMin  -2.      Is2= IsEmpty
 MAX-2INT  HI-2INT Timer DMin  HI-2INT  Is2= IsEmpty
 MAX-2INT MIN-2INT Timer DMin  MIN-2INT Is2= IsEmpty
 MIN-2INT MAX-2INT Timer DMin  MIN-2INT Is2= IsEmpty
 MIN-2INT  LO-2INT Timer DMin  MIN-2INT Is2= IsEmpty
 MAX-2INT       1. Timer DMin   1.      Is2= IsEmpty
 MAX-2INT      -1. Timer DMin  -1.      Is2= IsEmpty
 MIN-2INT       1. Timer DMin  MIN-2INT Is2= IsEmpty
 MIN-2INT      -1. Timer DMin  MIN-2INT Is2= IsEmpty

\ Header "2Swap", 0 ; ( a b c d -- c d a b )
1234 2345 3456 4567 5678 Timer 2Swap 2345 3456 Is2=  4567 5678 Is2=  1234 Is= IsEmpty

\ Header "Depth", 0 ; ( -- n ) number of items on parameter stack
110 220 330 Timer Depth 3 Is= 330 Is= 220 Is= 110 Is= IsEmpty

\ Header "Pick",0 ; ( xu...x1 x0 u -- xu...x1 x0 xu )
1111 2222 3333 1 Timer Pick 2222 Is= 3333 Is= 2222 Is= 1111 Is= IsEmpty

\ Header "Roll",0 ; ( xu xu-1 ... x0 u -- xu-1 ... x0 xu )
3333 2222 1111 0000 3 Timer Roll 3333 Is= 0 Is= 1111 Is= 2222 Is= IsEmpty

\ ------ LOGIC ----------

\ Header "Invert", 0 ; ( a -- ~a )
  0S  Timer Invert    1S  Is=  IsEmpty
  1S  Timer Invert    0S  Is=  IsEmpty
 1234 Timer Invert   edcb Is=  IsEmpty
$F0F0 Timer Invert  $0F0F Is=  IsEmpty

\ Header "True",0 ; ( -- f )
Timer True -1 Is= IsEmpty

\ Header "False",0 ; ( -- f )
Timer False 0 Is= IsEmpty

\ Header "And", 0 ; ( a b -- a&b )
 0        0 Timer And     0  Is=  IsEmpty
 0        1 Timer And     0  Is=  IsEmpty
 1        0 Timer And     0  Is=  IsEmpty
 1        1 Timer And     1  Is=  IsEmpty
 0 Invert 1 Timer And     1  Is=  IsEmpty
 1 Invert 1 Timer And     0  Is=  IsEmpty
 0S      0S Timer And    0S  Is=  IsEmpty
 0S      1S Timer And    0S  Is=  IsEmpty
 1S      0S Timer And    0S  Is=  IsEmpty
 1S      1S Timer And    1S  Is=  IsEmpty
 1234  5678 Timer And   1230 Is=  IsEmpty
$FF00 $0FF0 Timer And  $0F00 Is=  IsEmpty

\ Header "Or", 0 ; ( a b -- a|b )
  0S    0S  Timer Or    0S  Is=  IsEmpty
  0S    1S  Timer Or    1S  Is=  IsEmpty
  1S    0S  Timer Or    1S  Is=  IsEmpty
  1S    1S  Timer Or    1S  Is=  IsEmpty
 1234  5678 Timer Or   567c Is=  IsEmpty
$FF00 $0FF0 Timer Or  $FFF0 Is=  IsEmpty

\ Header "Xor", 0 ; ( a b -- a^b )
  0S    0S  Timer Xor    0S  Is=  IsEmpty
  0S    1S  Timer Xor    1S  Is=  IsEmpty
  1S    0S  Timer Xor    1S  Is=  IsEmpty
  1S    1S  Timer Xor    0S  Is=  IsEmpty
 1234  5678 Timer Xor   444c Is=  IsEmpty
$FF00 $0FF0 Timer Xor  $F0F0 Is=  IsEmpty


\ ------------ Arithmetic -------------------

\ Header "-", 0 ; ( a b -- a-b )
1025 255 Timer - 0DD0 Is= IsEmpty

\ Header "+", 0 ; ( a b -- a+b )
255 1025 Timer + 127A Is= IsEmpty

\ Header "Max",0 ; ( a b -- max )  signed
   0032    1032 Timer Max     1032 Is= IsEmpty
   0032    fedc Timer Max     0032 Is= IsEmpty
   1234    8032 Timer Max     1234 Is= IsEmpty
      0       1 Timer Max        1 Is= IsEmpty
      1       2 Timer Max        2 Is= IsEmpty
     -1       0 Timer Max        0 Is= IsEmpty
     -1       1 Timer Max        1 Is= IsEmpty
Min-Int       0 Timer Max        0 Is= IsEmpty
Min-Int Max-Int Timer Max  Max-Int Is= IsEmpty
      0 Max-Int Timer Max  Max-Int Is= IsEmpty
      0       0 Timer Max        0 Is= IsEmpty
      1       1 Timer Max        1 Is= IsEmpty
      1       0 Timer Max        1 Is= IsEmpty
      2       1 Timer Max        2 Is= IsEmpty
      0      -1 Timer Max        0 Is= IsEmpty
      1      -1 Timer Max        1 Is= IsEmpty
      0 Min-Int Timer Max        0 Is= IsEmpty
Max-Int Min-Int Timer Max  Max-Int Is= IsEmpty
Max-Int       0 Timer Max  Max-Int Is= IsEmpty

\ Header "Min",0 ; ( a b -- min )  signed
   0032    1032 Timer Min     0032 Is= IsEmpty
   0032    fedc Timer Min     fedc Is= IsEmpty
   1234    8032 Timer Min     8032 Is= IsEmpty
      0       1 Timer Min        0 Is= IsEmpty
      1       2 Timer Min        1 Is= IsEmpty
     -1       0 Timer Min       -1 Is= IsEmpty
     -1       1 Timer Min       -1 Is= IsEmpty
Min-Int       0 Timer Min  Min-Int Is= IsEmpty
Min-Int Max-Int Timer Min  Min-Int Is= IsEmpty
      0 Max-Int Timer Min        0 Is= IsEmpty
      0       0 Timer Min        0 Is= IsEmpty
      1       1 Timer Min        1 Is= IsEmpty
      1       0 Timer Min        0 Is= IsEmpty
      2       1 Timer Min        1 Is= IsEmpty
      0      -1 Timer Min       -1 Is= IsEmpty
      1      -1 Timer Min       -1 Is= IsEmpty
      0 Min-Int Timer Min  Min-Int Is= IsEmpty
Max-Int Min-Int Timer Min  Min-Int Is= IsEmpty
Max-Int       0 Timer Min        0 Is= IsEmpty

\ Header "S>D",0 ; ( n -- d )  Convert the signed number n to the double-cell number d
1234 Timer S>D 0000 Is= 1234 Is= IsEmpty
fedc Timer S>D ffff Is= fedc Is= IsEmpty

\ Header "U>D",0 ; ( u -- ud )  Convert the unsigned number n to the double-cell number d
1234 Timer U>D 0000 Is= 1234 Is= IsEmpty
fedc Timer U>D 0000 Is= fedc Is= IsEmpty

\ Header "D>S",0 ; ( d -- s )  Convert double to single
    1234  0 Timer D>S   1234   Is= IsEmpty
   -1234 -1 Timer D>S  -1234   Is= IsEmpty
 MAX-INT  0 Timer D>S  MAX-INT Is= IsEmpty
 MIN-INT -1 Timer D>S  MIN-INT Is= IsEmpty

\ Header "><",0 ; ( x1 -- x2 )  Swap byte order in cell
1234 Timer >< 3412 Is= IsEmpty

\ Header "Negate", 0 ; ( n -- -n )
ffe0 Timer Negate 0020 Is= IsEmpty
00e0 Timer Negate FF20 Is= IsEmpty

\ Header "Abs",0 ; ( n -- |n| )
ffe0 Timer Abs 0020 Is= IsEmpty
00e0 Timer Abs 00E0 Is= IsEmpty

\ Header "1-", 0 ; ( n -- n-1 )
0537 Timer 1- 0536 Is= IsEmpty
1300 Timer 1- 12ff Is= IsEmpty

\ Header "2+",0 ; ( n -- n+2 )
12ff Timer 2+ 1301 Is= IsEmpty

\ Header "1+", 0 ; ( n -- n+1 )
1032 Timer 1+ 1033 Is= IsEmpty
12ff Timer 1+ 1300 Is= IsEmpty

\ Header "DU2/",F_Inline ; ( ud -- ud/2 ) unsigned shift right
87654321. Timer DU2/  43b2a190. Is2= IsEmpty
76543210. Timer DU2/  3b2a1908. Is2= IsEmpty

\ Header "U2/",0 ; ( u -- u/2 ) unsigned shift right
0537 Timer U2/ 029B Is= IsEmpty
fedc Timer U2/ 7f6e Is= IsEmpty

\ Header "D2/",0 ; ( d -- d/2 ) signed shift right
       0. Timer D2/  0.        Is2= IsEmpty
       1. Timer D2/  0.        Is2= IsEmpty
      0 1 Timer D2/  MIN-INT 0 Is2= IsEmpty
 MAX-2INT Timer D2/  HI-2INT   Is2= IsEmpty
      -1. Timer D2/  -1.       Is2= IsEmpty
 MIN-2INT Timer D2/  LO-2INT   Is2= IsEmpty

\ Header "2/",0 ; ( n -- n/2 ) signed shift right
0537 Timer 2/ 029B Is= IsEmpty
fedc Timer 2/ ff6e Is= IsEmpty

\ Header "2*", 0 ; ( n -- n*2 )  shift left
0537 Timer 2* 0A6E Is= IsEmpty

\ Header "D2*",0 ; ( d -- d*2 )  double shift left\
              0. Timer D2*  0.             Is2= IsEmpty
 MIN-INT       0 Timer D2*  0 1            Is2= IsEmpty
         HI-2INT Timer D2*  MAX-2INT 1. D- Is2= IsEmpty
         LO-2INT Timer D2*  MIN-2INT       Is2= IsEmpty

\ Header "LShift",0 ; ( a u -- a<<u ) logical shift left
1234 4 Timer LShift 2340 Is=  IsEmpty
 #32 0 Timer LShift  #32 Is=  IsEmpty
 #32 3 Timer LShift #256 Is=  IsEmpty 

\ Header "RShift",0 ; ( a u -- a>>u ) logical shift right
1234 4 Timer RShift  0123 Is=  IsEmpty
#32  0 Timer RShift   #32 Is=  IsEmpty
#32  3 Timer RShift     4 Is=  IsEmpty

\ Header "UM*",0 ; ( u1 u2 -- ud ) unsigned 16x16 -> 32-bit result
      1025        255 Timer UM* 0025 Is=       A649 Is= IsEmpty
         0          0 Timer UM*  0.                Is2= IsEmpty
         0          1 Timer UM*  0.                Is2= IsEmpty
         1          0 Timer UM*  0.                Is2= IsEmpty
         1          2 Timer UM*  2.                Is2= IsEmpty
         2          1 Timer UM*  2.                Is2= IsEmpty
         3          3 Timer UM*  9.                Is2= IsEmpty
Mid-UInt+1 1 RSHIFT 2 Timer UM*  0 Is=   Mid-UInt+1 Is= IsEmpty
Mid-UInt+1          2 Timer UM*  1 Is=            0 Is= IsEmpty
Mid-UInt+1          4 Timer UM*  2 Is=            0 Is= IsEmpty
        1S          2 Timer UM*  1 Is=  1S 1 LShift Is= IsEmpty
  Max-UInt   Max-UInt Timer UM*  1 Invert Is=     1 Is= IsEmpty

\ Header "DNegate",0 ; ( d1 -- -d1 )  return -d1
             5678 1234 Timer DNegate edcb Is= a988 Is= IsEmpty
                    0. Timer DNegate   0. Is2=                   IsEmpty
                    1. Timer DNegate  -1. Is2=                   IsEmpty
                   -1. Timer DNegate   1. Is2=                   IsEmpty
              max-2int Timer DNegate  min-2int SWAP 1+ SWAP Is2= IsEmpty
 min-2int Swap 1+ Swap Timer DNegate  max-2int Is2=              IsEmpty

\ Header "DAbs",0 ; ( d -- ud )  ud is the absolute value of d
a988 edcb       Timer DAbs 1234 Is= 5678 Is= IsEmpty
5678 1234       Timer DAbs 1234 Is= 5678 Is= IsEmpty
       1.       Timer DAbs  1.       Is2=    IsEmpty
      -1.       Timer DAbs  1.       Is2=    IsEmpty
 MAX-2INT       Timer DAbs  MAX-2INT Is2=    IsEmpty
 MIN-2INT 1. D+ Timer DAbs  MAX-2INT Is2=    IsEmpty

\  Header "D+",0 ; ( d1 d2 -- d3 ) double precision add
              0.    5. Timer D+     5. Is2=         IsEmpty
             -5.    0. Timer D+    -5. Is2=         IsEmpty
              1.    2. Timer D+     3. Is2=         IsEmpty
              1.   -2. Timer D+    -1. Is2=         IsEmpty
             -1.    2. Timer D+     1. Is2=         IsEmpty
             -1.   -2. Timer D+    -3. Is2=         IsEmpty
             -1.    1. Timer D+     0. Is2=         IsEmpty
            0  0  0  5 Timer D+   0  5 Is2=         IsEmpty
           -1  5  0  0 Timer D+  -1  5 Is2=         IsEmpty
            0  0  0 -5 Timer D+   0 -5 Is2=         IsEmpty
            0 -5 -1  0 Timer D+  -1 -5 Is2=         IsEmpty
            0  1  0  2 Timer D+   0  3 Is2=         IsEmpty
           -1  1  0 -2 Timer D+  -1 -1 Is2=         IsEmpty
            0 -1  0  2 Timer D+   0  1 Is2=         IsEmpty
            0 -1 -1 -2 Timer D+  -1 -3 Is2=         IsEmpty
           -1 -1  0  1 Timer D+  -1  0 Is2=         IsEmpty
        MIN-INT 0 2DUP Timer D+  0 1 Is2=           IsEmpty
 MIN-INT S>D MIN-INT 0 Timer D+  0 0 Is2=           IsEmpty
      HI-2INT       1. Timer D+  0 HI-INT 1+ Is2=   IsEmpty
      HI-2INT     2DUP Timer D+  1S 1- MAX-INT Is2= IsEmpty
     MAX-2INT MIN-2INT Timer D+  -1. Is2=           IsEmpty
     MAX-2INT  LO-2INT Timer D+  HI-2INT Is2=       IsEmpty
      LO-2INT     2DUP Timer D+  MIN-2INT Is2=      IsEmpty
      HI-2INT MIN-2INT Timer D+ 1. D+  LO-2INT Is2= IsEmpty

\  Header "D-",0 ; ( d1 d2 -- d1-d2 ) double precision subtract
                0.  5. Timer D-             -5. Is2= IsEmpty
                5.  0. Timer D-              5. Is2= IsEmpty
                0. -5. Timer D-              5. Is2= IsEmpty
                1.  2. Timer D-             -1. Is2= IsEmpty
                1. -2. Timer D-              3. Is2= IsEmpty
               -1.  2. Timer D-             -3. Is2= IsEmpty
               -1. -2. Timer D-              1. Is2= IsEmpty
               -1. -1. Timer D-              0. Is2= IsEmpty
            0  0  0  5 Timer D-            0 -5 Is2= IsEmpty
           -1  5  0  0 Timer D-           -1  5 Is2= IsEmpty
            0  0 -1 -5 Timer D-            1  4 Is2= IsEmpty
            0 -5  0  0 Timer D-            0 -5 Is2= IsEmpty
           -1  1  0  2 Timer D-           -1 -1 Is2= IsEmpty
            0  1 -1 -2 Timer D-            1  2 Is2= IsEmpty
            0 -1  0  2 Timer D-            0 -3 Is2= IsEmpty
            0 -1  0 -2 Timer D-            0  1 Is2= IsEmpty
            0  0  0  1 Timer D-            0 -1 Is2= IsEmpty
        MIN-INT 0 2DUP Timer D-              0. Is2= IsEmpty
 MIN-INT S>D MAX-INT 0 Timer D-            1 1s Is2= IsEmpty
     MAX-2INT max-2INT Timer D-              0. Is2= IsEmpty
     MIN-2INT min-2INT Timer D-              0. Is2= IsEmpty
     MAX-2INT  hi-2INT Timer D- lo-2INT DNEGATE Is2= IsEmpty
      HI-2INT  lo-2INT Timer D-        max-2INT Is2= IsEmpty
      LO-2INT  hi-2INT Timer D-  min-2INT 1. D+ Is2= IsEmpty
     MIN-2INT min-2INT Timer D-              0. Is2= IsEmpty
     MIN-2INT  lo-2INT Timer D-         lo-2INT Is2= IsEmpty

\  Header "M+",0 ; ( d1 n -- d2 )  Add signed single to double
 HI-2INT   1 Timer M+  HI-2INT   1. D+ Is2= IsEmpty
 MAX-2INT -1 Timer M+  MAX-2INT -1. D+ Is2= IsEmpty
 MIN-2INT  1 Timer M+  MIN-2INT  1. D+ Is2= IsEmpty
 LO-2INT  -1 Timer M+  LO-2INT  -1. D+ Is2= IsEmpty

\ Header "UM*",0 ; ( u1 u2 -- ud ) unsigned 16x16 -> 32-bit result
 0                   0 Timer UM*  0.            Is2= IsEmpty
 0                   1 Timer UM*  0.            Is2= IsEmpty
 1                   0 Timer UM*  0.            Is2= IsEmpty
 1                   2 Timer UM*  2.            Is2= IsEmpty
 2                   1 Timer UM*  2.            Is2= IsEmpty
 3                   3 Timer UM*  9.            Is2= IsEmpty
 MID-UINT+1 1 RSHIFT 2 Timer UM*  MID-UINT+1 0  Is2= IsEmpty
 MID-UINT+1          2 Timer UM*  0 1           Is2= IsEmpty
 MID-UINT+1          4 Timer UM*  0 2           Is2= IsEmpty
         1S          2 Timer UM*  1S 1 LSHIFT 1 Is2= IsEmpty
   MAX-UINT   MAX-UINT Timer UM*  1 1 INVERT    Is2= IsEmpty

\ Header "UN*",0 ; ( ud1 u2 -- ud3 ) unsigned 32x16 -> 32-bit result
9876543. 0a Timer UN*  5f49f49e. Is2= IsEmpty

\ Header "*", 0 ; ( a b -- a*b ) 16x16 -> 16 (low word)
1025 0014 Timer * 42E4 Is= IsEmpty
 0     0  Timer *    0 Is= IsEmpty
 0     1  Timer *    0 Is= IsEmpty
 1     0  Timer *    0 Is= IsEmpty
 1     2  Timer *    2 Is= IsEmpty
 2     1  Timer *    2 Is= IsEmpty
 3     3  Timer *    9 Is= IsEmpty
-3     3  Timer *   -9 Is= IsEmpty
 3    -3  Timer *   -9 Is= IsEmpty
-3    -3  Timer *    9 Is= IsEmpty
Mid-UInt+1 1 RSHIFT 2 Timer *                Mid-UInt+1 Is= IsEmpty
Mid-UInt+1 2 RSHIFT 4 Timer *                Mid-UInt+1 Is= IsEmpty
Mid-UInt+1 1 RSHIFT Mid-UInt+1 OR 2 Timer *  Mid-UInt+1 Is= IsEmpty

\ Header "M*",0 ; ( a b -- dc ) 16x16 -> 32 signed
      0       0 Timer M*                   0 S>D Is2= IsEmpty
      0       1 Timer M*                   0 S>D Is2= IsEmpty
      1       0 Timer M*                   0 S>D Is2= IsEmpty
      1       2 Timer M*                   2 S>D Is2= IsEmpty
      2       1 Timer M*                   2 S>D Is2= IsEmpty
      3       3 Timer M*                   9 S>D Is2= IsEmpty
     -3       3 Timer M*                  -9 S>D Is2= IsEmpty
      3      -3 Timer M*                  -9 S>D Is2= IsEmpty
     -3      -3 Timer M*                   9 S>D Is2= IsEmpty
      0 Min-Int Timer M*                   0 S>D Is2= IsEmpty
      1 Min-Int Timer M*            Min-Int  S>D Is2= IsEmpty
      2 Min-Int Timer M*                   0 1S  Is2= IsEmpty
      0 Max-Int Timer M*                   0 S>D Is2= IsEmpty
      1 Max-Int Timer M*            Max-Int  S>D Is2= IsEmpty
      2 Max-Int Timer M*  Max-Int     1 LSHIFT 0 Is2= IsEmpty
Min-Int Min-Int Timer M*        0 MSB 1 RSHIFT   Is2= IsEmpty
Max-Int Min-Int Timer M*      MSB MSB 2/         Is2= IsEmpty
Max-Int Max-Int Timer M*        1 MSB 2/ Invert  Is2= IsEmpty

\ Header "UM/Mod", 0 ; ( ud u -- ur uq ) unsigned 32/16 -> 16 remainder, 16 quotient
    27c0         0009     000a Timer UM/Mod      EA60 Is=    0 Is= IsEmpty
       0            0        1 Timer UM/Mod         0 Is=    0 Is= IsEmpty
       1            0        1 Timer UM/Mod         1 Is=    0 Is= IsEmpty
       1            0        2 Timer UM/Mod         0 Is=    1 Is= IsEmpty
       3            0        2 Timer UM/Mod         1 Is=    1 Is= IsEmpty
MAX-UINT        2 UM*        2 Timer UM/Mod  Max-UInt Is=    0 Is= IsEmpty
MAX-UINT        2 UM* MAX-UINT Timer UM/Mod         2 Is=    0 Is= IsEmpty
MAX-UINT MAX-UINT UM* MAX-UINT Timer UM/Mod  Max-UInt Is=    0 Is= IsEmpty
    ffff         fffd     ffff Timer UM/Mod      fffe Is= fffd Is= IsEmpty

\ Header "UN/Mod",0 ; ( ud u -- udq ur )  unsigned 32/16 -> 32 quotient, 16 remainder
5678 1234   10 Timer UN/Mod   8 Is=  0123 Is=  4567 Is=  IsEmpty
ba98 fedc   10 Timer UN/Mod   8 Is=  0fed Is=  cba9 Is=  IsEmpty
0010 fffe ffff Timer UN/Mod  000f Is= 0000 Is= ffff Is=  IsEmpty
5f49f4a0.   0a Timer UN/Mod          2 Is= 9876543. Is2= IsEmpty

\ Header "SM/Rem ",0 ; ( d1 n1 -- n_remainder n_quotient )  Symmetric signed division
       0 S>D              1   Timer SM/Rem         0 Is=  0 Is= IsEmpty
       1 S>D              1   Timer SM/Rem         1 Is=  0 Is= IsEmpty
       2 S>D              1   Timer SM/Rem         2 Is=  0 Is= IsEmpty
      -1 S>D              1   Timer SM/Rem        -1 Is=  0 Is= IsEmpty
      -2 S>D              1   Timer SM/Rem        -2 Is=  0 Is= IsEmpty
       0 S>D             -1   Timer SM/Rem         0 Is=  0 Is= IsEmpty
       1 S>D             -1   Timer SM/Rem        -1 Is=  0 Is= IsEmpty
       2 S>D             -1   Timer SM/Rem        -2 Is=  0 Is= IsEmpty
      -1 S>D             -1   Timer SM/Rem         1 Is=  0 Is= IsEmpty
      -2 S>D             -1   Timer SM/Rem         2 Is=  0 Is= IsEmpty
       2 S>D              2   Timer SM/Rem         1 Is=  0 Is= IsEmpty
      -1 S>D             -1   Timer SM/Rem         1 Is=  0 Is= IsEmpty
      -2 S>D             -2   Timer SM/Rem         1 Is=  0 Is= IsEmpty
       7 S>D              3   Timer SM/Rem         2 Is=  1 Is= isEmpty
       7 S>D             -3   Timer SM/Rem        -2 Is=  1 Is= IsEmpty
      -7 S>D              3   Timer SM/Rem        -2 Is= -1 Is= IsEmpty
      -7 S>D              -3  Timer SM/Rem         2 Is= -1 Is= IsEmpty
 Max-Int  S>D              1  Timer SM/Rem   Max-Int Is=  0 Is= IsEmpty
 Min-Int  S>D              1  Timer SM/Rem   Min-Int Is=  0 Is= IsEmpty
 Max-Int  S>D        Max-Int  Timer SM/Rem         1 Is=  0 Is= IsEmpty
 Min-Int  S>D        Min-Int  Timer SM/Rem         1 Is=  0 Is= IsEmpty
      1S 1                 4  Timer SM/Rem   Max-Int Is=  3 Is= IsEmpty
       2 Min-Int  M*       2  Timer SM/Rem   Min-Int Is=  0 Is= IsEmpty
       2 Min-Int  M* Min-Int  Timer SM/Rem         2 Is=  0 Is= IsEmpty
       2 Max-Int  M*       2  Timer SM/Rem   Max-Int Is=  0 Is= IsEmpty
       2 Max-Int  M* Max-Int  Timer SM/Rem         2 Is=  0 Is= IsEmpty
 Min-Int  Min-Int  M* Min-Int Timer SM/Rem   Min-Int Is=  0 Is= IsEmpty
 Min-Int  Max-Int  M* Min-Int Timer SM/Rem   Max-Int Is=  0 Is= IsEmpty
 Min-Int  Max-Int  M* Max-Int Timer SM/Rem   Min-Int Is=  0 Is= IsEmpty
 Max-Int  Max-Int  M* Max-Int Timer SM/Rem   Max-Int Is=  0 Is= IsEmpty

\ Header "FM/Mod",0 ; ( d1 n1 -- n_remainder n_quotient )  Floored signed division
       0 S>D              1 Timer FM/Mod    0      Is=       0 Is= IsEmpty
       1 S>D              1 Timer FM/Mod    1      Is= 0 Is= IsEmpty
       2 S>D              1 Timer FM/Mod    2      Is= 0 Is= IsEmpty
      -1 S>D              1 Timer FM/Mod   -1      Is= 0 Is= IsEmpty
      -2 S>D              1 Timer FM/Mod   -2      Is= 0 Is= IsEmpty
       0 S>D             -1 Timer FM/Mod    0      Is= 0 Is= IsEmpty
       1 S>D             -1 Timer FM/Mod   -1      Is= 0 Is= IsEmpty
       2 S>D             -1 Timer FM/Mod   -2      Is= 0 Is= IsEmpty
      -1 S>D             -1 Timer FM/Mod    1      Is= 0 Is= IsEmpty
      -2 S>D             -1 Timer FM/Mod    2      Is= 0 Is= IsEmpty
       2 S>D              2 Timer FM/Mod    1      Is= 0 Is= IsEmpty
      -1 S>D             -1 Timer FM/Mod    1      Is= 0 Is= IsEmpty
      -2 S>D             -2 Timer FM/Mod    1      Is= 0 Is= IsEmpty
       7 S>D              3 Timer FM/Mod    2      Is= 1 Is= IsEmpty
       7 S>D             -3 Timer FM/Mod   -3     Is= -2 Is= IsEmpty
      -7 S>D              3 Timer FM/Mod   -3      Is= 2 Is= IsEmpty
      -7 S>D             -3 Timer FM/Mod    2     Is= -1 Is= IsEmpty
 MAX-INT S>D              1 Timer FM/Mod   MAX-INT Is= 0 Is= IsEmpty
 MIN-INT S>D              1 Timer FM/Mod   MIN-INT Is= 0 Is= IsEmpty
 MAX-INT S>D        MAX-INT Timer FM/Mod    1      Is= 0 Is= IsEmpty
 MIN-INT S>D        MIN-INT Timer FM/Mod    1      Is= 0 Is= IsEmpty
    1S 1                  4 Timer FM/Mod   MAX-INT Is= 3 Is= IsEmpty
       1 MIN-INT M*       1 Timer FM/Mod   MIN-INT Is= 0 Is= IsEmpty
       1 MIN-INT M* MIN-INT Timer FM/Mod    1      Is= 0 Is= IsEmpty
       2 MIN-INT M*       2 Timer FM/Mod   MIN-INT Is= 0 Is= IsEmpty
       2 MIN-INT M* MIN-INT Timer FM/Mod    2      Is= 0 Is= IsEmpty
       1 MAX-INT M*       1 Timer FM/Mod   MAX-INT Is= 0 Is= IsEmpty
       1 MAX-INT M* MAX-INT Timer FM/Mod    1      Is= 0 Is= IsEmpty
       2 MAX-INT M*       2 Timer FM/Mod   MAX-INT Is= 0 Is= IsEmpty
       2 MAX-INT M* MAX-INT Timer FM/Mod    2      Is= 0 Is= IsEmpty
 MIN-INT MIN-INT M* MIN-INT Timer FM/Mod   MIN-INT Is= 0 Is= IsEmpty
 MIN-INT MAX-INT M* MIN-INT Timer FM/Mod   MAX-INT Is= 0 Is= IsEmpty
 MIN-INT MAX-INT M* MAX-INT Timer FM/Mod   MIN-INT Is= 0 Is= IsEmpty
 MAX-INT MAX-INT M* MAX-INT Timer FM/Mod   MAX-INT Is= 0 Is= IsEmpty
\ Header "/Mod", 0 ; ( n1 n2 -- rem quot ) signed division
7fff 0a Timer /Mod 0CCC Is= 0007 Is= IsEmpty

\ Header "Mod",0 ; ( n1 n2 -- rem ) signed remainder
#1234 #10 Timer Mod 4 Is= IsEmpty
 
\ Header "/", 0 ; ( n1 n2 -- quot ) signed division
-7655 1234 Timer / -6 Is= IsEmpty

\ Header "*/Mod",0 ; ( n1 nmul ndiv -- nrem nquo )
#100 #10 #5 Timer */Mod #200 Is= 0 Is= IsEmpty
#100 #5 #10 Timer */Mod  #50 Is= 0 Is= IsEmpty
#101 #5 #10 Timer */Mod  #50 Is= 5 Is= IsEmpty

\ Header "*/",0 ; ( n1 nmul ndiv -- nquo )
#100 #10 #5 Timer */  #200 Is= IsEmpty
#100 #5 #10 Timer */   #50 Is= IsEmpty
#101 #5 #10 Timer */   #50 Is= IsEmpty

\ Header "U3*",0 ; ( ud1 u2 -- threecell )  unsigned multiply 32x16 -> 48
12345678. 1000 Timer U3* 123 Is= 45678000. Is2= IsEmpty

\ Header "N3*",0 ; ( d1 n2 -- threecell )  signed multiply 32x16 -> 48
12345678. 1000 Timer N3* 123 Is= 45678000. Is2= IsEmpty

\ Header "U3/Mod",0 ; ( u3 u -- udq ur )  unsigned 48/16 -> 32 quotient, 16 remainder
12345678. 9ab 1000 Timer U3/Mod 678 Is= 9ab12345. Is2= IsEmpty

\ Header "M*/",0 ; ( d1 n1 u2 -- d2 ) 
12345678. 4 40 Timer M*/ 1234567. Is2= IsEmpty

\ ---------- comparison ---------------

\ Header "Within",0 ; ( n1|u1 n2|u2 n3|u3 -- flag )  Is n1 within n2..n3
10 5 15 Timer Within  True  Is= IsEmpty
-4 5 15 Timer Within  False Is= IsEmpty

\ Header "0<>",0 ; ( n -- flag )
   1 Timer 0<> True  Is= IsEmpty
   0 Timer 0<> False Is= IsEmpty
8000 Timer 0<> True  Is= IsEmpty

\ Header "0=",0 ; ( n -- flag )
   1 Timer 0=  False Is=  IsEmpty
   0 Timer 0=  True  Is=  IsEmpty
8000 Timer 0=  False Is=  IsEmpty
  32 Timer 0=  False Is=  IsEmpty

\ Header "0<",0 ; ( n -- flag )
   1 Timer 0<  False Is=  IsEmpty
   0 Timer 0<  False Is=  IsEmpty
  -1 Timer 0<  True  Is=  IsEmpty
8000 Timer 0<  True  Is=  IsEmpty
7fff Timer 0<  False Is=  IsEmpty
  32 Timer 0<  False Is=  IsEmpty
 -42 Timer 0<  True  Is=  IsEmpty

\ Header "0>",0 ; ( n -- flag )
   1 Timer 0>  True  Is=  IsEmpty
   0 Timer 0>  False Is=  IsEmpty
  -1 Timer 0>  False Is=  IsEmpty
8000 Timer 0>  False Is=  IsEmpty
7fff Timer 0>  True  Is=  IsEmpty
  32 Timer 0>  True  Is=  IsEmpty
 -42 Timer 0>  False Is=  IsEmpty
   0 Timer 0>  False Is=  IsEmpty

\ Header "=", 0 ; ( a b -- flag )
1233 1234 Timer =  False Is=  IsEmpty
1334 1234 Timer =  False Is=  IsEmpty
1234 1234 Timer =  True  Is=  IsEmpty
 #32  #32 Timer =  True  Is=  IsEmpty
 #42  #32 Timer =  False Is=  IsEmpty

\ Header "<>", 0 ; ( a b -- flag )
1233 1234 Timer <>  True  Is=  IsEmpty
1334 1234 Timer <>  True  Is=  IsEmpty
1234 1234 Timer <>  False Is=  IsEmpty
  32   32 Timer <>  False Is=  IsEmpty
  42   32 Timer <>  True  Is=  IsEmpty

\ Header "<",0 ; ( a b -- flag ) signed
      0       1 Timer <  True  Is=  IsEmpty
      1       2 Timer <  True  Is=  IsEmpty
     -1       0 Timer <  True  Is=  IsEmpty
     -1       1 Timer <  True  Is=  IsEmpty
Min-Int       0 Timer <  True  Is=  IsEmpty
Min-Int Max-Int Timer <  True  Is=  IsEmpty
      0 Max-Int Timer <  True  Is=  IsEmpty
      0       0 Timer <  False Is=  IsEmpty
      1       1 Timer <  False Is=  IsEmpty
      1       0 Timer <  False Is=  IsEmpty
      2       1 Timer <  False Is=  IsEmpty
      0      -1 Timer <  False Is=  IsEmpty
      1      -1 Timer <  False Is=  IsEmpty
      0 Min-Int Timer <  False Is=  IsEmpty
Max-Int Min-Int Timer <  False Is=  IsEmpty
Max-Int       0 Timer <  False Is=  IsEmpty
     32      32 Timer <  False Is=  IsEmpty
     42      32 Timer <  False Is=  IsEmpty
     32      42 Timer <  True  Is=  IsEmpty
     32     -42 Timer <  False Is=  IsEmpty

\ Header ">",0 ; ( a b -- flag ) signed
      0       1 Timer >  False Is=  IsEmpty
      1       2 Timer >  False Is=  IsEmpty
     -1       0 Timer >  False Is=  IsEmpty
     -1       1 Timer >  False Is=  IsEmpty
Min-Int       0 Timer >  False Is=  IsEmpty
Min-Int Max-Int Timer >  False Is=  IsEmpty
      0 Max-Int Timer >  False Is=  IsEmpty
      0       0 Timer >  False Is=  IsEmpty
      1       1 Timer >  False Is=  IsEmpty
      1       0 Timer >  True  Is=  IsEmpty
      2       1 Timer >  True  Is=  IsEmpty
      0      -1 Timer >  True  Is=  IsEmpty
      1      -1 Timer >  True  Is=  IsEmpty
      0 Min-Int Timer >  True  Is=  IsEmpty
Max-Int Min-Int Timer >  True  Is=  IsEmpty
Max-Int       0 Timer >  True  Is=  IsEmpty
     32      32 Timer >  False Is=  IsEmpty
     42      32 Timer >  True  Is=  IsEmpty
     32      42 Timer >  False Is=  IsEmpty
     32     -42 Timer >  True  Is=  IsEmpty

\ Header "U<",0 ; ( u1 u2 -- flag ) unsigned less than
       0        1  Timer U<  True  Is=  IsEmpty
       1        2  Timer U<  True  Is=  IsEmpty
       0 Mid-UInt  Timer U<  True  Is=  IsEmpty
       0 Max-UInt  Timer U<  True  Is=  IsEmpty
Mid-UInt  Max-UInt Timer U<  True  Is=  IsEmpty
       0        0  Timer U<  False Is=  IsEmpty
       1        1  Timer U<  False Is=  IsEmpty
       1        0  Timer U<  False Is=  IsEmpty
       2        1  Timer U<  False Is=  IsEmpty
Mid-UInt         0 Timer U<  False Is=  IsEmpty
Max-UInt         0 Timer U<  False Is=  IsEmpty
Max-UInt  Mid-UInt Timer U<  False Is=  IsEmpty
      32        32 Timer U<  False Is=  IsEmpty
      42        32 Timer U<  False Is=  IsEmpty
      32        42 Timer U<  True  Is=  IsEmpty
      32      fffe Timer U<  True  Is=  IsEmpty

\ Header "U>",0 ; ( u1 u2 -- flag ) unsigned greater than
       0        1  Timer U>  False Is=  IsEmpty
       1        2  Timer U>  False Is=  IsEmpty
       0 Mid-UInt  Timer U>  False Is=  IsEmpty
       0 Max-UInt  Timer U>  False Is=  IsEmpty
Mid-UInt  Max-UInt Timer U>  False Is=  IsEmpty
       0        0  Timer U>  False Is=  IsEmpty
       1        1  Timer U>  False Is=  IsEmpty
       1        0  Timer U>  True  Is=  IsEmpty
       2        1  Timer U>  True  Is=  IsEmpty
Mid-UInt         0 Timer U>  True  Is=  IsEmpty
Max-UInt         0 Timer U>  True  Is=  IsEmpty
Max-UInt  Mid-UInt Timer U>  True  Is=  IsEmpty
      32        32 Timer U>  False Is=  IsEmpty
      42        32 Timer U>  True  Is=  IsEmpty
      32        42 Timer U>  False Is=  IsEmpty
      32      fffe Timer U>  False Is=  IsEmpty

\ Header "D0=",0 ; ( d -- flag )
               1. Timer D0=  <FALSE> Is= IsEmpty
 MIN-INT        0 Timer D0=  <FALSE> Is= IsEmpty
         MAX-2INT Timer D0=  <FALSE> Is= IsEmpty
      -1  MAX-INT Timer D0=  <FALSE> Is= IsEmpty
               0. Timer D0=  <TRUE>  Is= IsEmpty
              -1. Timer D0=  <FALSE> Is= IsEmpty
       0  MIN-INT Timer D0=  <FALSE> Is= IsEmpty

\ Header "D0<",0 ; ( d -- flag )
                0. Timer D0<  <FALSE> Is= IsEmpty
                1. Timer D0<  <FALSE> Is= IsEmpty
  MIN-INT        0 Timer D0<  <FALSE> Is= IsEmpty
        0  MAX-INT Timer D0<  <FALSE> Is= IsEmpty
          MAX-2INT Timer D0<  <FALSE> Is= IsEmpty
               -1. Timer D0<  <TRUE>  Is= IsEmpty
          MIN-2INT Timer D0<  <TRUE>  Is= IsEmpty

\ Header "D=",0 ; ( d1 d2 -- flag ) double equal
      -1.      -1. Timer D=  <TRUE>  Is= IsEmpty
      -1.       0. Timer D=  <FALSE> Is= IsEmpty
      -1.       1. Timer D=  <FALSE> Is= IsEmpty
       0.      -1. Timer D=  <FALSE> Is= IsEmpty
       0.       0. Timer D=  <TRUE>  Is= IsEmpty
       0.       1. Timer D=  <FALSE> Is= IsEmpty
       1.      -1. Timer D=  <FALSE> Is= IsEmpty
       1.       0. Timer D=  <FALSE> Is= IsEmpty
       1.       1. Timer D=  <TRUE>  Is= IsEmpty
   0   -1    0  -1 Timer D=  <TRUE>  Is= IsEmpty
   0   -1    0   0 Timer D=  <FALSE> Is= IsEmpty
   0   -1    0   1 Timer D=  <FALSE> Is= IsEmpty
   0    0    0  -1 Timer D=  <FALSE> Is= IsEmpty
   0    0    0   0 Timer D=  <TRUE>  Is= IsEmpty
   0    0    0   1 Timer D=  <FALSE> Is= IsEmpty
   0    1    0  -1 Timer D=  <FALSE> Is= IsEmpty
   0    1    0   0 Timer D=  <FALSE> Is= IsEmpty
   0    1    0   1 Timer D=  <TRUE>  Is= IsEmpty
 MAX-2INT MIN-2INT Timer D=  <FALSE> Is= IsEmpty
 MAX-2INT       0. Timer D=  <FALSE> Is= IsEmpty
 MAX-2INT MAX-2INT Timer D=  <TRUE>  Is= IsEmpty
 MAX-2INT HI-2INT  Timer D=  <FALSE> Is= IsEmpty
 MAX-2INT MIN-2INT Timer D=  <FALSE> Is= IsEmpty
 MIN-2INT MIN-2INT Timer D=  <TRUE>  Is= IsEmpty
 MIN-2INT LO-2INT  Timer D=  <FALSE> Is= IsEmpty
 MIN-2INT MAX-2INT Timer D=  <FALSE> Is= IsEmpty

\ Header "DU<",0 ; ( ud1 ud2 -- flag )
       1.       1. Timer DU<  <FALSE> Is= IsEmpty
       1.      -1. Timer DU<  <TRUE>  Is= IsEmpty
      -1.       1. Timer DU<  <FALSE> Is= IsEmpty
      -1.      -2. Timer DU<  <FALSE> Is= IsEmpty
 MAX-2INT  HI-2INT Timer DU<  <FALSE> Is= IsEmpty
  HI-2INT MAX-2INT Timer DU<  <TRUE>  Is= IsEmpty
 MAX-2INT MIN-2INT Timer DU<  <TRUE>  Is= IsEmpty
 MIN-2INT MAX-2INT Timer DU<  <FALSE> Is= IsEmpty
 MIN-2INT  LO-2INT Timer DU<  <TRUE>  Is= IsEmpty

\ Header "D<",0 ; ( d1 d2 -- flag )
       0.       1.    Timer D<  <TRUE>  Is= IsEmpty
       0.       0.    Timer D<  <FALSE> Is= IsEmpty
       1.       0.    Timer D<  <FALSE> Is= IsEmpty
      -1.       1.    Timer D<  <TRUE>  Is= IsEmpty
      -1.       0.    Timer D<  <TRUE>  Is= IsEmpty
      -2.      -1.    Timer D<  <TRUE>  Is= IsEmpty
      -1.      -2.    Timer D<  <FALSE> Is= IsEmpty
      -1. MAX-2INT    Timer D<  <TRUE>  Is= IsEmpty
 MIN-2INT MAX-2INT    Timer D<  <TRUE>  Is= IsEmpty
 MAX-2INT      -1.    Timer D<  <FALSE> Is= IsEmpty
 MAX-2INT MIN-2INT    Timer D<  <FALSE> Is= IsEmpty
 MAX-2INT 2DUP -1. D+ Timer D<  <FALSE> Is= IsEmpty
 MIN-2INT 2DUP  1. D+ Timer D<  <TRUE>  Is= IsEmpty

\ -------------- Return stack ----------------------

\ Header ">R", 0 ; ( a -- ) (R: -- a )  push to return stack
\ Header "R@", 0 ; ( -- a ) (R: a -- a )  get a copy of top of return stack
\ Header "R>", 0 ; ( -- a ) (R: a -- )  pop from return stack
: TestR1 ( -- )
  1221 >R 2332 >R IsEmpty
  R@ 2332 Is= IsEmpty
  R> 2332 Is= R> 1221 Is= IsEmpty
  765 >R RDrop IsEmpty
  ;  IsEmpty  SeeLatest
Timer TestR1 IsEmpty

\ Header "2>R",0 ; ( x1 x2 -- ) ( R: -- x1 x2 )  move cell pair to return stack
\ Header "2R@",0 ; ( R: x1 x2 -- x1 x2 ) ( -- x1 x2 )  Get top 2 return stack cells
\ Header "2R>",0 ; ( R: x1 x2 -- ) ( -- x1 x2 )  Pop 2 cells from the return stack
: TestR2 ( -- )
  12345678. 2>R  'A' Emit  23456789. 2>R 'B' Emit  IsEmpty
  2R@ 'C' Emit 23456789. Is2= IsEmpty
  2R> 'D' Emit 23456789. Is2= 2R> 'E' Emit 12345678. Is2= IsEmpty
  ;  IsEmpty  SeeLatest
Timer TestR2 IsEmpty

\ ----- Memory ------------------------

\ Header "Variable",0 ; ( "name" -- )  Define a variable word
Variable V1  0 ,  IsEmpty  SeeLatest \ 2Variable
Timer V1 4 + Here Is= IsEmpty

\ Header "@",0 ; ( addr -- val ) fetch cell
\ Header "!", 0 ; ( val addr -- ) store cell
1234 V1 Timer !  IsEmpty
V1 Timer @ 1234 Is= IsEmpty

\ Header "C@", 0 ; ( addr -- byte ) fetch byte
\ Header "C!", 0 ; ( byte addr -- ) store byte
56 V1 Timer C! IsEmpty
V1 Timer C@ 56 Is= IsEmpty

\ Header "2@", 0 ; ( addr -- d ) fetch double cell
\ Header "2!", 0 ; ( d addr -- ) store double cell
2345 6789 V1 Timer 2! IsEmpty
V1 @ 6789 Is=  V1 2+ @ 2345 Is=  IsEmpty
V1 Timer 2@ 2345 6789 Is2= IsEmpty 

\ Header "+!",ha_Inline ; ( n|u a-addr -- )  Add n | u to the single-cell number at a-addr
4321 V1 Timer +!  IsEmpty  V1 @ aaaa Is= IsEmpty

\ Header "?",0 ; ( a-addr -- )  Display the cell value stored at a-addr.
V1 ? IsEmpty

Create SBuf  12 C, 34 C, 56 C,
Create FBuf  0 , 0 ,
\ Header "Fill", 0 ; ( caddr u byte -- ) fill u bytes starting at addr with byte
FBUF 0 20 Timer Fill IsEmpty
  FBuf C@ 0 Is=  FBuf 1+ C@ 0 Is=  FBuf 2+ C@ 0 Is=  FBuf 3 + C@ 0 Is=
\ Header "Blank",0 ; ( c-addr u -- )  Fill with blanks
FBUF 1 Timer Blank IsEmpty
  FBuf C@ 20 Is=  FBuf 1+ C@ 00 Is=  FBuf 2+ C@ 00 Is=  FBuf 3 + C@ 0 Is=
FBUF 3 Timer Blank IsEmpty
  FBuf C@ 20 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=  FBuf 3 + C@ 0 Is=
\ Header "Erase",0 ; ( c-addr u -- )  Fill with zeros
FBuf 1 Timer Erase IsEmpty
  FBuf C@ 0 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=  FBuf 3 + C@ 0 Is=

\ Header "CMove", 0 ; ( src dst u -- ) copy u bytes from src to dst
FBUF 3 Timer Blank IsEmpty
FBUF FBUF 3 Timer CMove IsEmpty
  FBuf C@ 20 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=  FBuf 3 + C@ 0 Is=
SBUF FBUF 0 Timer CMove IsEmpty
  FBuf C@ 20 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=  FBuf 3 + C@ 0 Is=
SBUF FBUF 1 Timer CMove IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 20 Is=  FBuf 2+ C@ 20 Is=  FBuf 3 + C@ 0 Is=
SBUF FBUF 3 Timer CMove IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 34 Is=  FBuf 2+ C@ 56 Is=  FBuf 3 + C@ 0 Is=
FBUF FBUF 1+ 2 Timer CMove IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 12 Is=  FBuf 2+ C@ 12 Is=  FBuf 3 + C@ 0 Is=
SBUF FBUF 3 Timer CMove IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 34 Is=  FBuf 2+ C@ 56 Is=  FBuf 3 + C@ 0 Is=
FBUF 1+ FBUF 2 Timer CMove IsEmpty
  FBuf C@ 34 Is=  FBuf 1+ C@ 56 Is=  FBuf 2+ C@ 56 Is=  FBuf 3 + C@ 0 Is=

\ Header "CMove>",0 ; ( src dest u -- ) copy u bytes from src to dst descending
SBuf FBuf 3 Timer CMove> IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 34 Is=  FBuf 2+ C@ 56 Is=  FBuf 3 + C@ 0 Is=
FBuf FBuf 1+ 2 Timer CMove> IsEmpty 
  FBuf C@ 12 Is=  FBuf 1+ C@ 12 Is=  FBuf 2+ C@ 34 Is=  FBuf 3 + C@ 0 Is= 
SBuf FBuf 3 Timer CMove> IsEmpty
FBuf 1+ FBuf 2 Timer CMove> IsEmpty
  FBuf C@ 56 Is=  FBuf 1+ C@ 56 Is=  FBuf 2+ C@ 56 Is=  FBuf 3 + C@ 0 Is= 

\ Header "Move",0 ; ( src dst ucount -- )  Move count address units from src to dst
SBuf FBuf 3 Timer Move IsEmpty
  FBuf C@ 12 Is=  FBuf 1+ C@ 34 Is=  FBuf 2+ C@ 56 Is=  FBuf 3 + C@ 0 Is=
FBuf 1+ FBuf 2 Timer Move IsEmpty
  FBuf C@ 34 Is=  FBuf 1+ C@ 56 Is=  FBuf 2+ C@ 56 Is=  FBuf 3 + C@ 0 Is= 
SBuf FBuf 3 Timer Move IsEmpty
FBuf FBuf 1+ 2 Timer Move IsEmpty 
  FBuf C@ 12 Is=  FBuf 1+ C@ 12 Is=  FBuf 2+ C@ 34 Is=  FBuf 3 + C@ 0 Is= 

\ -------- Console I/O ------------------

\ Header "Key", 0 ; ( -- char ) receive character (blocking)
\ Header "Key?", 0 ; ( -- flag ) non-blocking check for available input
\ Header "Emit", 0 ; ( char -- ) transmit character
45 Emit IsEmpty

\ Header "CR",0 ; ( -- ) emit new line
CR IsEmpty

\ Header "Space",0 ; ( -- ) emit a space
Space IsEmpty  45 Emit IsEmpty

\ Header "Spaces",0 ; ( n -- ) emit n spaces
4 Spaces IsEmpty 45 Emit IsEmpty

\ Header "Type",0 ; ( addr u -- ) transmit u characters from addr
S" abxy" Dup 4 Is=  Type IsEmpty

\ Header "Accept",0 ; ( bufaddr buflen -- actualLen ) read a line from console into buffer

\ Header ".Hex", 0 ; ( n -- ) Print TOS as hex
1234 .Hex IsEmpty

\ Header "C.Hex",0 ; ( n -- ) Print TOS as 2-digit hex
56 C.Hex IsEmpty

\ Header "Sign",0 ; ( n -- ) If n is negative, prepend a '-' to the pictured numeric output string
\ Header "<#",0 ; ( -- )  init pictured numeric output
\ Header "#>",0 ; ( d -- adr len )  finish pictured numeric output
\ Header "Hold",0 ; ( char -- )  prepend char to pictured numeric output string
\ Header "#",0 ; ( d -- d )  convert 1 digit
\ Header "#S",0 ; ( d -- d )  convert all digits

\ Header "DU.",0 ; ( ud -- )  print unsigned double
12345678. DU. IsEmpty

\ Header "U.",0 ; ( u -- )  print as unsigned number
fedc U. IsEmpty '>' Emit

\ Header ".", 0 ; ( n -- ) print signed number
fedc . IsEmpty '>' Emit

\ Header "D.",0 ; ( d -- )  print double signed number
fedcba98. D.  IsEmpty '>' Emit

\ Header "U.R",0 ; ; ( uval nchars -- ) print unsigned number in nchars chars
1234 5 U.R IsEmpty '>' Emit

\ Header ".R",0 ; ( nval nchars -- ) print signed number in nchars chars
fedc 8 .R IsEmpty '>' Emit

\ Header "D.R",0 ; ( d n -- )  print right aligned in n chars
fedcba98. 12 D.R  IsEmpty '>' Emit

\ -------------------------------------------

\ Header "Execute",0 ; ( xt -- ) execute word by execution token
1234 ' 2+ Timer Execute 1236 Is= IsEmpty

\ Header ">Body",0 ; ( xt -- adr )
' Execute Timer >Body 3 - ' Execute Is= IsEmpty

\ Header "Exit",F_Immediate ; ( -- ) compile return from current colon definition
: TestExit
  2345 Exit 3456 ;  SeeLatest  IsEmpty
TestExit 2345 Is= IsEmpty

\ Header "Ahead",F_Immediate ; 

\ Header "If",F_Immediate ; ( -- patch_addr )  Compile an If
\ Header "Else",F_Immediate ; ( patch_addr -- patch2_addr ) Compile "Else"
\ Header "Then",F_Immediate ; ( patch_addr -- )  Compile "Then"
: IfTest If 1111 Else 2222 Then ; SeeLatest IsEmpty
   1 Timer IfTest 1111 Is= IsEmpty
   0 Timer IfTest 2222 Is= IsEmpty
edcb Timer IfTest 1111 Is= IsEmpty
: GI1 If 123 Then ; SeeLatest IsEmpty
 0 Timer GI1  IsEmpty
 1 Timer GI1  123 Is= IsEmpty
-1 Timer GI1  123 Is= IsEmpty
: GI2 If 123 Else 234 Then ; SeeLatest IsEmpty
 0 Timer GI2  234 Is= IsEmpty
 1 Timer GI2  123 Is= IsEmpty
-1 Timer GI2  123 Is= IsEmpty
: TEST-IF1 IF 99 THEN ; SeeLatest IsEmpty
 1 Timer TEST-IF1  99 Is=  IsEmpty
 0 Timer TEST-IF1          IsEmpty
: TEST-IF3 IF 99 ELSE 42 THEN ; SeeLatest IsEmpty
 1 Timer TEST-IF3  99 Is= IsEmpty
 0 Timer TEST-IF3  42 Is= IsEmpty
: TEST-DOUBLE-IF  IF 1. D- THEN ; SeeLatest IsEmpty
 1. 1 Timer TEST-DOUBLE-IF  0. Is2=  IsEmpty
 3. 1 Timer TEST-DOUBLE-IF  2. Is2=  IsEmpty
: TEST-DOUBLE-IFELSE  IF 1. ELSE 2. THEN ; SeeLatest IsEmpty
1 Timer TEST-DOUBLE-IFELSE  1. Is2=  IsEmpty

\ Header "Begin",F_Immediate ; ( -- rev_addr )  Compile "Begin"
\ Header "Again",F_Immediate ; ( rev_addr -- )  Compile "Again"
: AgainTest
  1 Begin  1+  Dup .  Swap Over + Swap  Over 5 > If Drop Exit Then  Again ;  SeeLatest IsEmpty
0 Timer AgainTest 9 Is= IsEmpty

\ Header "Until",F_Immediate ; ( rev_addr -- )  Compile "Until"
: UntilTest
  1 Begin  1+  Dup .  Swap Over + Swap  Over 5 > Until Drop ;  SeeLatest IsEmpty
0 Timer UntilTest 9 Is= IsEmpty
: GI4 Begin Dup 1+ Dup 5 > Until ; SeeLatest IsEmpty
3 Timer GI4
  6 Is= 5 Is= 4 Is= 3 Is= IsEmpty
5 Timer GI4
  6 Is= 5 Is= IsEmpty
6 Timer GI4
  7 Is= 6 Is= IsEmpty

\ Header "While",F_Immediate ; ( rev_addr -- rev_addr fwd_addr )  Compile "While"
\ Header "Repeat",F_Immediate ; ( rev_addr fwd_addr -- )  Compile "Repeat"
: WhileTest
  1 Begin  1+  Dup 6 < While  Swap Over + Swap  Repeat  Drop ; SeeLatest IsEmpty
0 Timer WhileTest 0e Is= IsEmpty
: GI3 Begin Dup 5 < While Dup 1+ Repeat ; SeeLatest IsEmpty
0 Timer GI3
  5 Is= 4 Is= 3 Is= 2 Is= 1 Is= 0 Is= IsEmpty
4 Timer GI3
  5 Is= 4 Is= IsEmpty
5 Timer GI3
  5 Is= IsEmpty
6 Timer GI3
  6 Is= IsEmpty


\ Header "Do",F_Immediate ; ( -- back_addr )  Compile a DO
\ Header "Loop",F_Immediate ; ( back-addr -- )  Compile LOOP
: LoopTest
  6 1 Do  I +  Loop ; SeeLatest IsEmpty
20 Timer LoopTest 2f Is= IsEmpty
: GD1 Do I Loop ; SeeLatest IsEmpty
         4        1 Timer GD1  3 Is= 2 Is= 1 Is= IsEmpty
         2       -1 Timer GD1  1 Is= 0 Is= -1 Is= IsEmpty
Mid-UInt+1 Mid-UInt Timer GD1  Mid-UInt Is= IsEmpty
: TEST-DOUBLE-DO 0. 3 0 DO 1. D+ LOOP ; SeeLatest IsEmpty
Timer TEST-DOUBLE-DO  3. Is2= IsEmpty

\ Header "+Loop",F_Immediate ; ( back-addr -- )  Compile +Loop
: +LoopTest1  6 1 do  I +  2 +Loop ; SeeLatest IsEmpty
20 Timer +LoopTest1 29 Is= IsEmpty
: +LoopTest2  0 5 Do  I +  -2 +Loop ; SeeLatest IsEmpty
20 Timer +LoopTest1 29 Is= IsEmpty

VARIABLE gditerations  SeeLatest
VARIABLE gdincrement  SeeLatest IsEmpty
: gd7 ( limit start increment -- )
   gdincrement !
   0 gditerations !
   DO
     1 gditerations +!
     I dup .
     gditerations @ 6 = IF ( LEAVE ) Unloop gditerations @ Exit THEN
     gdincrement @
   +LOOP gditerations @
  ;  SeeLatest IsEmpty
Decimal
   4  4  -1 Timer gd7   1 Is=  4 Is= IsEmpty
   1  4  -1 Timer gd7   4 Is= 1 Is= 2 Is= 3 Is= 4 Is= IsEmpty
   4  1  -1 Timer gd7   6 Is= -4 Is= -3 Is= -2 Is= -1 Is= 0 Is= 1 Is= IsEmpty
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
MAX-UINT 8 RSHIFT 1+ Constant ustep
ustep NEGATE Constant -ustep
MAX-INT 7 RSHIFT 1+ Constant step
step NEGATE Constant -step
VARIABLE bump
: gd8 bump ! DO 1+ bump @ +LOOP ; SeeLatest IsEmpty
 0 MAX-UINT 0      ustep Timer gd8  256 Is= IsEmpty
 0 0 MAX-UINT     -ustep Timer gd8  256 Is= IsEmpty
 0 MAX-INT MIN-INT  step Timer gd8  256 Is= IsEmpty
 0 MIN-INT MAX-INT -step Timer gd8  256 Is= IsEmpty
Hex

\ ; Header "?Do",F_Immediate ; 

\ Header "Unloop",F_Immediate ; ( -- ) (R: limit index -- ) discard DO loop parameters
: UnloopTest ( n -- n' )
  5 1 Do  I 3 > If  Unloop Exit Then  I + Loop ;  SeeLatest  IsEmpty
10 Timer UnloopTest 16 Is= IsEmpty
: GD6 ( PAT: {0 0},{0 0}{1 0}{1 1},{0 0}{1 0}{1 1}{2 0}{2 1}{2 2} )
   0 SWAP 0 DO
      I 1+ 0 DO
        I J + 3 = IF I UNLOOP I UNLOOP EXIT THEN 1+
      LOOP
   LOOP ; SeeLatest IsEmpty
1 Timer GD6  .S  1 Is= IsEmpty
2 Timer GD6  .S  3 Is= IsEmpty
3 Timer GD6  .S  2 Is= 1 Is= 4 Is= IsEmpty

\ ; Header "Leave",F_Immediate ; ( -- )

\ Header "I",0 ; ( -- n ) (R: limit index -- limit index) copy loop index
\ Header "J",0 ; ( -- n ) ( R: 2limit 2index 1limit 1index ) copy 2nd loop index
: IJTest ( n -- n' )
  5 1 Do  3 0 Do  I J * Dup . +  Loop  Loop ; SeeLatest IsEmpty
Decimal
20 Timer IJTest 50 Is= IsEmpty
Hex

\ Header "Defer",0 ; ( "name" -- )  Define a defer word
\ Header "Defer!",0 ; ( xt2 xt1 -- )  set the defer word xt1 to execute xt2
\ Header "Defer@",0 ; ( xt1 -- xt2 )  return the xt that defer xt1 executes
\ Header "Action-Of",F_Immediate ; ( "name" -- xt ) xt is the execution token that name is set to execute
\ Header "Is",F_Immediate ; ( xt "name" -- ) Set defer name to execute xt
DEFER defer2  IsEmpty SeeLatest
\ defer2  \ deferred word is uninitialized
\ RECOVERED?  -1 Is= IsEmpty

' * ' defer2 DEFER! IsEmpty  ' defer2 8 Dump
2 3 Timer defer2 6 Is= IsEmpty
' + IS defer2 IsEmpty  ' defer2 8 Dump
1 2 Timer defer2 3 Is= IsEmpty
: DT1 ['] - Is defer2 ; SeeLatest IsEmpty
Timer DT1 IsEmpty
1 2 Timer defer2 -1 Is= IsEmpty
' defer2 Defer@ ' - Is= IsEmpty
Action-Of defer2 ' - Is= IsEmpty
: DT2 Action-Of defer2 ; SeeLatest IsEmpty
DT2 ' - Is= IsEmpty

: RETURNS42 42 ; SeeLatest IsEmpty
' RETURNS42 Timer IS defer2  IsEmpty
Timer defer2  42 Is= IsEmpty

: SET-MYDEFER ['] RETURNS42 IS defer2 ;  SeeLatest IsEmpty
: RETURNS99 99 ;  SeeLatest IsEmpty
' RETURNS99 Timer IS defer2 IsEmpty
Timer defer2  99 Is= IsEmpty
Timer SET-MYDEFER IsEmpty
Timer defer2  42 Is= IsEmpty

' defer2 Timer DEFER@  ' RETURNS42 Is= IsEmpty 

Timer ACTION-OF defer2  ' RETURNS42 Is= IsEmpty

: GET-ACTION ACTION-OF defer2 ;  SeeLatest IsEmpty
Timer GET-ACTION  ' RETURNS42 Is= IsEmpty

' RETURNS99 ' defer2 Timer DEFER!  IsEmpty
Timer defer2  99 Is= IsEmpty

\ IS safety check - not a DEFER
\ 55 CONSTANT NotADefer
\ ' RETURNS42 Timer IS NotADefer  IsEmpty \ not a DEFER
\ RECOVERED?  -1 Is= IsEmpty 

\ ACTION-OF safety check - not a DEFER
\ ACTION-OF NotADefer IsEmpty \ not a DEFER
\ RECOVERED?  -1 Is= IsEmpty


\ Header "Case",F_Immediate ; ( -- endlink nextlink )
\ Header "EndCase",F_Immediate ; ( endlink -- )
\ Header "Of",F_Immediate ; ( endlink -- endlink nextlink )
\ Header "EndOf",F_Immediate ; ( endlink nextlink -- endlink )

Here .Hex
: cs1
  CASE
     1 OF 111 ENDOF
     2 OF 222 ENDOF
     3 OF 333 ENDOF
     >R 999 R>
    ENDCASE
  ; IsEmpty SeeLatest
1 Timer cs1  111 Is= IsEmpty
2 Timer cs1  222 Is= IsEmpty
3 Timer cs1  333 Is= IsEmpty
4 Timer cs1  999 Is= IsEmpty
: cs2
  >R CASE
    -1 OF
      CASE R@
        1 OF 100 ENDOF
        2 OF 200 ENDOF
        >R -300 R>
       ENDCASE
     ENDOF
    -2 OF
      CASE R@
        1 OF -99 ENDOF
        >R -199 R>
       ENDCASE
     ENDOF
    >R 299 R>
   ENDCASE
  R> DROP ;  IsEmpty SeeLatest

-1 1 Timer cs2   100 Is= IsEmpty
-1 2 Timer cs2   200 Is= IsEmpty
-1 3 Timer cs2  -300 Is= IsEmpty
-2 1 Timer cs2   -99 Is= IsEmpty
-2 2 Timer cs2  -199 Is= IsEmpty
 0 2 Timer cs2   299 Is= IsEmpty

\ -------- dictionary ---------------------------

\ Header "Chars",0 ; ( n1 -- n2 )
123 Timer Chars 123 Is= IsEmpty

\ Header "Char+",0 ; ( n1 -- n2 )  add 1 char in address units
123 Timer Char+ 124 Is= IsEmpty

\ Header "Cell",0 ; ( -- n )  size of a cell in address units
Timer Cell 2 Is= IsEmpty

\ Header "Cells",0 ; ( n1 -- n2 )  convert cells to address units
123 Timer Cells 246 Is= IsEmpty

\ Header "Cell+",0 ; ( n1 -- n2 )  add 1 cell in address units
123 Timer Cell+ 125 Is= IsEmpty

\ Header "Align",0 ; ( -- )  Align dictionary pointer (DP) to next even address
Here Timer Align Here Swap - Dup . 2 U< True Is= IsEmpty

\ Header "Aligned",0 ; ( addr1 -- addr2 )  Align address to even
2345 Timer Aligned 2346 Is= IsEmpty

\ Header "Unused",0 ; ( -- u )  return the space remaining in the region addressed by HERE
Timer Unused 1500 U> True Is= IsEmpty

\ Header "Here",0 ; ( -- addr ) current dictionary pointer
Timer Here .Hex IsEmpty

\ Header "Pad",0 ; ( -- c-addr )  Return transient region
Timer Pad Here - . IsEmpty

\ Header "Buffer:",0 ; ( u "<spaces>name" -- )  create named buffer
7 Timer Buffer: Buf1 IsEmpty
Here 7 - Timer Buf1 Is= IsEmpty

\ Header "Allot",0 ; ( n -- ) advance dictionary pointer by n bytes
Here 5 +  5 Timer Allot  Here Is= IsEmpty

\ Header ",",0 ; ( val -- ) compile cell into dictionary
Here  4321 Timer ,  Here - -2 Is= IsEmpty
Here 2 - @ 4321 Is= IsEmpty

\ Header "C,",0 ; ( byte -- ) compile byte into dictionary
Here 56 Timer C, Here - -1 Is= IsEmpty
Here 1- C@ 56 Is= IsEmpty

\ Header "2,",0 ; ( d -- )  compile double into dictionary
Here  98765432. Timer 2,
2@ 98765432. Is2= IsEmpty

\ Header "Postpone",F_Immediate ; ( "<spaces>name" -- )
: GT1 123 ; IsEmpty SeeLatest
: GT4 POSTPONE GT1 ; IMMEDIATE IsEmpty SeeLatest
: GT5 GT4 ; SeeLatest Depth . IsEmpty
GT5 123 Is= IsEmpty
: GT6 345 ; IMMEDIATE IsEmpty SeeLatest
: GT7 POSTPONE GT6 ; IsEmpty SeeLatest
GT7 345 Is= IsEmpty

\ Header "Recurse",F_Immediate ; ( -- )  call the current word
: Recur1 ( N -- 0,1,..N )
  DUP IF DUP >R 1- RECURSE R> THEN ;  SeeLatest
2 Recur1 2 Is= 1 Is= 0 Is= IsEmpty

\ Header "[Compile]",F_Immediate ; ( "name" -- )  compile call to name
: TestCompile2 [Compile] 2+ ; IsEmpty SeeLatest
5432 TestCompile2 5434 Is= IsEmpty
: [c1] [COMPILE] Dup ; Immediate SeeLatest IsEmpty
123 Timer [c1]  123 Is=  123 Is=  IsEmpty

\ Header "Compile,",0 ; ( xt -- )  Compile a jsr abs
: TestCompile, [ ' 1+ Compile, ] ; IsEmpty SeeLatest
2345 TestCompile, 2346 Is= IsEmpty

\ Header "Jmp,",0 ; ( xt -- )  compile a jmp abs

\ Header "Ld#,",0 ; ( n -- )  compile lda #

\ Header "Ldd#",0 ; ( d -- ) compile ldy # : lda #

\ Header "PushA,",0 ; ( -- ) compile PushA

\ Header "Literal",F_Immediate ; ( n -- )  Compile inline Constant
\ Header "2Literal",F_Immediate ; ( d -- )  Compile inline double constant
: LitTest  789 3456789. ;  SeeLatest IsEmpty
Timer LitTest 3456789. Is2= 789 Is= IsEmpty

\ Header "Char",0 ; ( "<spaces>name" -- char ) Put the value of its first character onto the stack.
IsEmpty
Timer Char D 'D' Is= Depth . IsEmpty

\ Header "[Char]",F_Immediate ; ( "c" -- n )  compile char literal
: Chr1 [Char] E ; IsEmpty SeeLatest
Timer Chr1 'E' Is= IsEmpty

\ ; Header "String,",0  ; ( caddr len xt -- ) compile string

\ Header "Header,",0 ; ( addr len -- )  Compile a word header

\ Header "Traverse-WordList",0 ; ( i*x xt wid -- j*x )  call xt for each word in wid
Variable TW1 IsEmpty
: TWSub ( xt -- ) 1 TW1 +! Space Name>String Type ; IsEmpty SeeLatest
' Dup TWSub IsEmpty
0 TW1 !
' TWSub Latest @ Timer Traverse-WordList IsEmpty
  TW1 @ Dup . 0> True Is= IsEmpty

\ Header "Immediate",F_Immediate ; ( -- ) set immediate flag on latest word
' TWSub 3 - C@ 5 Is= IsEmpty
Timer Immediate IsEmpty
' TWSub 3 - C@ $85 Is= IsEmpty

\ Header "Name>Compile",0 ; ( nt -- x xt )  Get compilation info for nt
' Rot Timer Name>Compile  ' Rot Is=  ' CompileNt, Is=  IsEmpty
' Do  Timer Name>Compile  ' Do  Is=  ' Execute    Is=  IsEmpty

\ Header "Name>String",0 ; ( nt -- c-addr u )  Given a name token, return name as a string
' Dup Timer Name>String Type IsEmpty
' Dup Timer Name>String 3 Is= ' Dup 6 - Is= IsEmpty

\ Header "Words",0 ; ( -- )  Walk dictionary and print names
Words IsEmpty

\ Header "Find",0 ; ( c-addr -- c-addr 0 | xt 1 | xt -1 )
Create FBuf 1 C, '+' C,
FBuf Timer Find 1 Is= ' + Is= IsEmpty
'_' FBuf 1+ C!
FBuf Timer Find 0 Is=  FBuf Is= IsEmpty

\ Header "Search-WordList",0 ; ( caddr u wid -- 0 | xt 1 | xt -1)  "Search for a word in a wordlist"

\ Header "'",0 ; ( "name" -- xt )  find a word
Timer ' Nip Name>String 3 Is= C@ 'N' Is= IsEmpty

\ Header "[']",F_Immediate ; ( "<spaces>name" -- )  ' as a literal
: TickTest ['] Dup ; IsEmpty  SeeLatest
Timer TickTest ' Dup Is= IsEmpty

\ Header "Constant",0 ; ( "name" n -- )  Define a Constant word
abcd Timer Constant K1  IsEmpty
Timer K1 abcd Is= IsEmpty
55   Timer Constant Limit  IsEmpty
Timer Limit  55 Is=  IsEmpty

\ Header "2Constant",0 ; ( "name" d -- )  Define a Constant double word
12345678. Timer 2Constant K2     IsEmpty
  654321. Timer 2Constant TestK  IsEmpty
Timer K2     12345678. Is2=  IsEmpty
Timer TestK    654321. Is2=  IsEmpty

\ Header "Variable",0 ; ( "name" -- )  Define a variable word
Timer Variable Var1  IsEmpty
Here 2 - Timer Var1 Is= IsEmpty
Var1 @ 0 Is= IsEmpty
3456 Var1 ! IsEmpty
Var1 @ 3456 Is= IsEmpty

\ Header "2Variable",0 ; ( "name" -- )  Define a double variable word
Timer 2Variable Var2  IsEmpty
Here 4 - Timer Var2 Is= IsEmpty
Var2 2@ 0. Is2= IsEmpty
98765432. Var2 2! IsEmpty
Var2 2@ 98765432. Is2= IsEmpty

\ Header "Value",0 ; ( x "name" -- )  Define a value word
\ Header "2Value",0 ; ( d "name" -- )  Define a 2value word
\ Header "To",F_Immediate ; ( x "name" -- )  Set a value word
2345 Timer Value Val1 IsEmpty
Timer Val1 2345 Is= IsEmpty
7654 Timer To Val1  IsEmpty
Timer Val1 7654 Is= IsEmpty

23456789. Timer 2Value Val2 IsEmpty
Timer Val2 23456789. Is2= IsEmpty
76543210. Timer To Val2  IsEmpty
Timer Val2 76543210. Is2= IsEmpty

\ ; Header "Smudge",0 ; ( -- )

\ Header "Create",0 ; ( "name" -- )  Create a word that pushes the addr of it's parameter field
\ Header "Does>", F_Immediate ;
: abc Create , Does> @ ;  SeeLatest IsEmpty
4321 Timer abc def  IsEmpty
Timer def 4321 Is= IsEmpty
: KONS Create ,  Does> @ ; SeeLatest  IsEmpty
55 Timer KONS KLimit  IsEmpty
Timer KLimit  55 Is=  IsEmpty

\ Header ".(",F_Immediate ; ( "ccc<paren>" -- )  type string
Timer .( testing2 ) IsEmpty

\ Header '."',F_Immediate ; ( string" -- )  Type a string literal
." Testing1"
: ."Test ." Testing2" ;  SeeLatest IsEmpty
."Test

\ Header 'S"',F_Immediate ; ( -- caddr len )  create a string literal
Timer S" test3" Dup 5 Is=  Type  IsEmpty
: S"Test  S" test3" ; SeeLatest IsEmpty
Timer S"Test Dup 5 Is=  Type IsEmpty


\ Header 'C"',F_Immediate ; ( 
C" ABC"   Dup C@ 3 Is=  Dup 1+ C@ 'A' Is=  3 + C@ 'C' Is=  IsEmpty
C" xyzt"  Dup C@ 4 Is=  Dup 1+ C@ 'x' Is=  4 + C@ 't' Is=  IsEmpty
: CStr1 C" testing" ;  IsEmpty  SeeLatest
: CStr2 C" hello" ;  IsEmpty  SeeLatest
Timer CStr1  Count  7 Is=  Dup C@ 't' Is=  6 + C@ 'g' Is=  IsEmpty
Timer CStr2  Count  5 Is=  Dup C@ 'h' Is=  4 + C@ 'o' Is=  IsEmpty


\ Header "MustBeCompiling",F_Immediate ; ( -- ) make sure we're compiling
: TestMBC  MustBeCompiling  ;  SeeLatest  IsEmpty
TestMBC  IsEmpty

\ Header 'Abort"', F_Immediate ; If f is true, print string & abort
: TestAbort"  Abort" test" ;  IsEmpty  SeeLatest
0 Timer TestAbort"

\ -----------------------------------------------

\ Header "User0",0 ; ( -- addr )  Return addr of user area
Timer User0 .Hex IsEmpty

\ Header "Latest",0 ; ( -- addr ) address of LATEST variable in user area
Timer Latest User0 - $80 U< True Is= IsEmpty
' TestAbort" Latest @ Is= IsEmpty

\ Header "LatestXt",0 ; ( -- addr ) address of LATESTXT variable in user area
Timer LatestXt User0 - $80 U< True Is= IsEmpty

\ Header "Base",0 ; ( -- addr ) address of BASE variable
Timer Base User0 - $80 U< True Is= IsEmpty
Base @ 10 Is= IsEmpty

\ Header "State",0 ; ( -- addr ) address of STATE variable
Timer State User0 - $80 U< True Is= IsEmpty
State @ 0 Is= IsEmpty

\ Header ">In",0 ; ( -- addr ) address of >IN variable
Timer >In User0 - $80 U< True Is= IsEmpty
>In @ 6 Is= IsEmpty

\ Header "Scr",0 ; ( -- adr ) variable
Timer Scr User0 - $80 U< True Is= IsEmpty

\ Header "Blk",0 ; ( -- adr ) variable
Timer Blk User0 - $80 U< True Is= IsEmpty

\ Header "Source-Id",0 ; ( -- adr ) variable
Timer Source-Id User0 - $80 U< True Is= IsEmpty

\ Header "RandState",0 ; ( -- adr )
Timer RandState User0 - $80 U< True Is= IsEmpty

\ Header "FSP",0 ; ( -- adr )
Timer FSP  User0 - $80 U< True Is= IsEmpty

\ Header "FsFull",0 ; ( -- adr )
Timer FSFull User0 - $80 U< True Is= IsEmpty

\ Header "FSEmpty+1",0 ; ( -- adr )
Timer FSEmpty+1  User0 - $80 U< True Is= IsEmpty

\ Header "Temp0",0 ; ( -- adr ) Push addr of Temp0
Timer Temp0 User0 - $ff80 And 0 Is= IsEmpty

\ Header "Source",0 ; ( -- addr len ) current input source
Timer Source Type IsEmpty

\ Header "PStack",0 ; ( -- n )  direct-page offset of param stack
Timer PStack $ff80 And 0 Is= IsEmpty

\ Header "Decimal",0 ; ( -- )  set base to 10
4 Base !  Timer Decimal  Base @ Hex     0a Is= IsEmpty

\ Header "Hex",0 ; ( -- )  set base to 16
4 Base !  Timer Hex      Base @ Decimal 16 Is= IsEmpty
Hex

\ ----------------------------------------

\ Header "Bounds",0 ; ( addr u -- addr+u addr ) Prepare address for looping

Create CntBuf  3 C,
\ Header "Count",0 ; ( addr -- addr+1 len ) counted string to addr/len
CntBuf Timer Count  3 Is=  Here Is= IsEmpty

: s1 ( -- adr len )  S" abcde" ;  SeeLatest IsEmpty
: s6 ( -- adr len )  S" abb" ;  SeeLatest IsEmpty
\ Header "Compare",0 ; ( c-addr1 u1 c-addr2 u2 -- n )  Compare 2 strings
: s11 S" 0abc" ; SeeLatest
: s12 S" 0aBc" ; SeeLatest
s11 s12 Timer Compare   1 Is= IsEmpty
s12 s11 Timer Compare  -1 Is= IsEmpty

\ Header "(",F_Immediate ; (  ccc(paren)  -- )  Comment
34 Timer ( this is a comment ) 12
12 Is= 34 Is= IsEmpty

\ Header "\",F_Immediate ; ( -- )  Eat remainder of parse line as a comment

: "abdde"  S" abdde"  ;  SeeLatest IsEmpty
: "abbde"  S" abbde"  ;  SeeLatest IsEmpty
: "abcdf"  S" abcdf"  ;  SeeLatest IsEmpty
: "abcdee" S" abcdee" ;  SeeLatest IsEmpty
s1 "abcdee" Timer Compare   1 Is= IsEmpty
s1 "abdde"  Timer Compare  -1 Is= IsEmpty
s1 "abbde"  Timer Compare   1 Is= IsEmpty
s1 "abcdf"  Timer Compare  -1 Is= IsEmpty

s1        s1 Timer Compare  0 Is= IsEmpty
s1  Pad Swap Timer CMove  IsEmpty		\ Copy s1 to PAD
s1  Pad Over Timer Compare  0 Is= IsEmpty
s1     Pad 6 Timer Compare  1 Is= IsEmpty
Pad 10    s1 Timer Compare -1 Is= IsEmpty
\ s1     Pad 0 Timer Compare  1 Is= IsEmpty
\ Pad  0    s1 Timer Compare -1 Is= IsEmpty
s1        s6 Timer Compare  1 Is= IsEmpty
s6        s1 Timer Compare -1 Is= IsEmpty

\ ; Header "-Trailing",0 ; ( addr n1 -- addr n2 )  trim trailing spaces
\ ; Header "Upper",0 ; ( addr len -- )  Convert chars to uppercase

\ Header "Parse",0 ; ( "name" c -- addr u )  "Parse input with delimiter character"
\ Header "Parse-Name",0 ; (  (spaces)name(space)  -- c-addr u )  Skip leading spaces. Parse space delimited name.
\ Header ">Number",0 ; ( ud1 c-addr1 u1 -- ud2 c-addr2 u2 )
\ Header "Number",0 ; ( adr len -- 0 ) or ( adr len -- n -1 ) or ( adr len -- d -2 ) or ( adr len -- fp -3 )

\ Header "Bl",0 ; ( -- n ) ASCII value of space (blank)
Timer Bl $20 Is= IsEmpty

\ ----- blocks --------------------------------------------------

\ Header "RamDiskWrite",0 ; ( adr block -- stat )  Write a 512 byte block

\ Header "RamDiskRead",0 ; ( adr block -- stat )  read a 512 byte block

\ Header "BlockAdd",0 ; ( u -- )  add blocks to system

\ Header "Block",0 ; ( u -- a-addr ) return address of block buffer filled with block u.

\ Header "Buffer",0 ; ( u -- a-addr ) return address of block buffer assigned to block u.

\ Header "Empty-Buffers",0 ; ( -- ) Unassign all block buffers. No disk writes. 

\ Header "Flush",0 ; ( -- )

\ Header "Save-Buffers",0 ; ( -- ) Save UPDATEd block buffers. Mark as unmodified

\ Header "Update",0 ; ( -- ) Mark the current block buffer as modified. 

\ Header "List",0 ; ( u -- ) Display block u. Store u in SCR

\ Header "Load",0 ; ( u -- )  Evalute the contents of block u

\ Header "Thru",0 ; ( u1 u2 -- ) LOAD blocks u1 through u2 in sequence

\ ---- system -------------------------------------------------------

\ Header "[Defined]",F_Immediate ; ( "<spaces>name ..." -- flag )
Timer [Defined] Dup True  Is= IsEmpty
Timer [Defined] jak False Is= IsEmpty

\ Header "[Undefined]",F_Immediate ; ( "<spaces>name ..." -- flag )
Timer [Undefined] Dup False Is= IsEmpty
Timer [Undefined] jak True  Is= IsEmpty

\ Header "Forget",0 ; ( "name" -- )  Forget dictionary entries back to & including "name".

\ Header "Marker",0 ; ( "<spaces>name" -- ) Create a definition for name that deletes itself 

\ Header "Environment?",0 ; ( caddr u -- ?? )
s" abc" Timer Environment? False Is= IsEmpty

\ Header "SeeLatest",0 ; ( -- )  show code of latest word

\ Header "Cold",0 ; Cold start
\ Header "Bye",0 ; ( -- ) halt the system
\ Header "Abort",0 ; ( -- ) reset stacks and go to QUIT
\ Header "Quit", 0 ; ( -- ) outer interpreter loop
\ Header "Interpret",0 ; ( -- ) parse and execute/compile words from input

\ Header ".S",0 ; ( -- ) print stack contents non-destructively
.s  IsEmpty
111 222 333 .s  333 Is= 222 Is= 111 Is= IsEmpty

\ Header "Dump",0 ; ( caddr len -- )  Dump memory in hex
$400 $82 Dump IsEmpty

\ Header "SeeLatest",0 ; ( -- )  show code of latest word

\ Header "[",F_Immediate ; ( -- )  switch State to interpret
\ Header "]",0 ; ( -- )  switch State to compile
Decimal
: []Test  3 [ 7 ] Literal * ; SeeLatest IsEmpty
[]Test 21 Is= IsEmpty
Hex

\ Header "Code",0 ; ( "name" -- )  Start a machine code word
\ Header ";Code",F_Immediate ; ( -- )  End a machine code word

\ Header ":", 0 ; ( "name" -- )  start compiling a new colon word
\ Header ";",F_Immediate ;  finish compiling a new colon word
: Foo 42 ;  SeeLatest  IsEmpty
Timer Foo  42 Is=  IsEmpty

\ Header ":NoName",0 ; ( -- xt ) start compiling a new colon word, with no header
:NoName   49 ;  SeeLatest
Dup .Hex
Execute  49 Is=  IsEmpty

\ Header ";Code",F_Immediate ; ( -- )  End a machine code word
\ Header "Code",0 ; ( "name" -- )  Start a machine code word

\ Header "Abort",0 ; ( -- ) reset stacks and go to QUIT
\ Header "Quit",0 ; ( -- ) outer interpreter loop

\ Header "Refill",0 ; ( -- flag )  Attempt to fill the input buffer from the input source

\ Header "Restore-Input",0 ; ( )
\ Header "Save-Input",0 ; ( )
\ Header "Evaluate",0 ; ( addr len -- )  interpret a string
: GE1 S" 123" ; IMMEDIATE IsEmpty SeeLatest
: GE2 S" 123 1+" ; IMMEDIATE IsEmpty SeeLatest
: GE3 S" : GE4 345 ;" ; IsEmpty SeeLatest
: GE5 EVALUATE ; IMMEDIATE SeeLatest
GE1 Timer EVALUATE  123 Is= IsEmpty ( TEST EVALUATE IN INTERP. STATE )
GE2 Timer EVALUATE  124 Is= IsEmpty
GE3 Timer EVALUATE IsEmpty  SeeLatest
Timer GE4          345 Is= IsEmpty

: GE6 GE1 GE5 ; IsEmpty SeeLatest ( TEST EVALUATE IN COMPILE STATE )
Timer GE6  123 Is= IsEmpty
: GE7 GE2 GE5 ; IsEmpty SeeLatest
Timer GE7  124 Is= IsEmpty

\ Done!
