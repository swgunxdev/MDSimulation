/*
Copyright (c) 2010 - 2012 Jordan "Earlz/hckr83" Earls  <http://lastyearswishes.com>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.
   
THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL
THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR
OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

.assembly extern mscorlib
{
  .ver 2:0:0:0
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 ) // .z\V.4..
}
.assembly 'BareMetal'
{
  .custom instance void class [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::'.ctor'() =  (
                01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01       ) // ceptionThrows.

  .hash algorithm 0x00008004
  .ver  1:0:0:0
}
.module BareMetal.dll 


.namespace Earlz.BareMetal
{
  .class public auto ansi abstract sealed beforefieldinit BareMetal
        extends [mscorlib]System.Object
  {
    .method public static hidebysig
           default int32 SizeOf<T> ()  cil managed
    {
        .maxstack 1
	sizeof !!T
          ret
    }

  }
}

