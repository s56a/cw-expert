PROGRAM FFT_PAS;

    {$S+} {$I+} {$N+}

    USES
       CRT, DOS;

    CONST
       rate=8000;
       FFTlen=64;
       F2L=128;
       noise=4;
       aver=10;
       bwl=3;
       bwh=16;

    VAR
       m: array [0..f2l,0..4] of real;
       la, da: array[0..32] of byte;
       r: word;
       ix, lb, db, b: byte;
       ch: CHAR;
       key: BOOLEAN;
       s,morse: string[63];
       output: array [0..f2l] of STRING;
       sig,sum,ave: array [0..f2l] of Integer;
       re0, im0, re1, re2, im1, im2, RealF, ImagF, Mag, prag: ARRAY [0..f2l] OF Real;
       broj: array [0..f2l] of byte;
       si, co, wd: ARRAY [0..f2l] OF Real;
       tim: array [0..f2l] of longint;
       v, thd, Period: Real;
       t,ctr: longint;
       inp: file of word;
       log: text;
       dup,cls: array [0..210] of string[6];
       okc, clc, j, n, n1, w, x, y, z, l, loopend, dotmin: INTEGER;
       bitrev, old1, old2: array [0..f2l] of byte;
       keyes: array [0..f2l] of boolean;


    PROCEDURE Init;

       BEGIN
          assign(inp,'kc21.wav');
          reset(inp);
          ctr:=0;
          okc:=0;
          clc:=0;
          dup[0]:='      ';
          assign(log,'kc988.txt');
          {$I-} RESET(log); {$I+}
          if ioresult=0 then begin
             repeat
                readln(log,s);
                cls[clc]:=s;
                inc(clc);
                until eof(log);
             close(log);
             end
          else writeln('No calls 98');
          assign(log,'kc998.txt');
          {$I-} RESET(log); {$I+}
          if ioresult=0 then begin
             repeat
                readln(log,s);
                cls[clc]:=s;
                inc(clc);
                until eof(log);
             close(log);
             end
          else writeln('No calls 99');
          writeln(clc-1,' calls');
          l:=Round(Ln(f2l)/Ln(2));
          Period:=f2l/rate;
          FOR n:=0 TO f2l-1 DO BEGIN
             x:=n;
             Y:=0;
             N1:=f2l;
             FOR W:=1 TO L DO BEGIN
                N1:=N1 ShR 1;
                IF X>=N1 THEN BEGIN
                   IF W=1 THEN Inc(Y)
                   ELSE Y:=Y+(2 ShL (W-2));
                   X:=X-N1;
                   END;
                END;
             BitRev[n]:=y;
             END;
          dotmin:=round(0.03/period);  { 40 wpm, 30 msec dot}
          morse:='ETIANMSURWDKGOHVF*L*PJBXCYZQ**54*3***2 ******16*/*****7***8*90*';
          for n:=0 to f2l do begin
             prag[n]:=noise;
             ave[n]:=aver;
             sum[n]:=0;
             tim[n]:=1;
             old1[n]:=0;
             old2[n]:=0;
             keyes[n]:=false;
             output[n]:='';
             broj[n]:=0;
             for j:=0 to 4 do m[n,j]:=1;
             end;
          v:=2*Pi/f2l;
          FOR n:=0 TO f2l-1 DO BEGIN
             RealF[n]:=0;
             ImagF[n]:=0;
             si[n]:=-Sin(n*v);
             co[n]:= Cos(n*v);
             wd[n]:=(0.54 + 0.46 * co[n])/f2l;
             END;
          END;


    Procedure cw2asc;

       BEGIN
          IF sum[z] IN [1..63] THEN ch:=morse[sum[z]]
          ELSE IF sum[z]=75 THEN ch:='?'
          ELSE ch:='*';
          IF sum[z]>0 THEN output[z]:=output[z]+ch;
          sum[z]:=0;
          END;


    Procedure Filter;

       var loc: integer;
           prn: boolean;

       begin
          s:=output[z];
          output[z]:='';
          prn:=(length(s)>3) AND (Pos('*',s) = 0);
          IF s[1] IN ['2'..'9'] THEN loc:=3 else loc:=1;
          if prn THEN repeat
             prn:=s[loc] in ['0'..'9'];
             inc(loc);
             until prn or (loc>length(s));
          loc:=0;
          if prn and (clc>0) then repeat
             prn:=s=cls[loc];
             Inc(loc);
             until prn or (loc>clc);
          if prn then BEGIN
             j:=0;
             while (j<okc) and (s<>dup[j]) do inc(j);
             if (s<>dup[j]) then begin
                writeln(okc+1:3,ctr:6,z:6,round(prag[z]):6,ave[z]:6,'  ',s);
                dup[j]:=s;
                inc(okc);
                end;
             end;
          end;


    Procedure CWdecode;

       begin
          keyes[z]:=key;
          tim[z]:=ctr;
          if NOT key then begin
             Inc(sum[z]);
             if t>ave[z] then inc(sum[z])
             else if t>dotmin then ave[z]:=t + (ave[z] div 2);
             end
          else if t<ave[z] then begin
             if t>dotmin then ave[z]:=t + (ave[z] div 2);
             sum[z]:=2*sum[z];
             if sum[z]>75 then sum[z]:=0;
             end
          end;

    Function Median: real;

      var
          min, k: integer;
          window: array [0..4] of real;
          temp: real;

      Begin
      for j := 0 to 4 do window[j] := m[z,j];
      for j := 0 to 2 do begin
         min := j;
         for k:= j + 1 to 4 do
            if (window[k] < window[min]) then min := k;
         temp := window[j];
         window[j] := window[min];
         window[min] := temp;
         end;
      Median := window[2];
      end;


    PROCEDURE Sig2Sym;

       BEGIN
             for j:=0 to 3 do m[z,j]:=m[z,j+1];
             m[z,4]:= mag[z];
             v:=Median;
             prag[z]:=(31*prag[z]+v)/32;
             thd:=(prag[z]+noise)/2;
             if thd<noise then thd:=noise;
             b:=broj[z];
             if v>thd then begin
                if b<3 then inc(b)
                end
             else if b>0 then dec(b);
             broj[z]:=b;
             key:=b>1;
             t:=ctr-tim[z];
             if key<>keyes[z] then CWDecode
             else if NOT key THEN BEGIN
                if (t = ave[z]) then CW2ASC;
                if (t=2*ave[z]) then Filter;
                end;
             end;


    PROCEDURE CalcFFT;

       VAR
          i,k,m,mx,I1,I2,I3,I4,I5: INTEGER;
          A1,A2,B1,B2,Z1,Z2: Real;

       BEGIN
          I1:=f2l div 2;
          I2:=1;
          FOR I:=1 TO L DO BEGIN
             I3:=0;
             I4:=I1;
             FOR K:=1 TO I2 DO BEGIN
                x:=I3 DIV I1;
                i5:=BitRev[x];
                z1:=Co[i5];
                z2:=Si[i5];
                LoopEnd:=I4-1;
                FOR M:=I3 TO LoopEnd DO BEGIN
                   A1:=RealF[M];
                   A2:=ImagF[M];
                   Mx:=M+I1;
                   B1:=Z1*RealF[Mx]-Z2*ImagF[Mx];
                   B2:=Z2*RealF[Mx]+Z1*ImagF[Mx];
                   RealF[M]:=(A1+B1);
                   ImagF[M]:=(A2+B2);
                   RealF[Mx]:=(A1-B1);
                   ImagF[Mx]:=(A2-B2);
                   END;
                I3:=I3+(I1 ShL 1);
                I4:=I4+(I1 ShL 1);
                END;
             I1:=I1 ShR 1;
             I2:=I2 ShL 1;
             END;
          END;


    PROCEDURE Samples;

       BEGIN
          FOR n:=0 TO FFTlen-1 DO BEGIN
             if eof(inp) then r:=1
             else read(inp,r);
             RealF[n+FFTlen]:=wd[n+FFTlen]*old1[n];
             old1[n] := r div 256;
             RealF[n]:=wd[n]*old1[n];
             ImagF[n+FFTlen]:=wd[n+fftlen]*old2[n];
             old2[n] := r mod 256;
             ImagF[n]:=wd[n]*old2[n];
             end;
          end;


    Procedure Spectra;

       Begin
          for n:=bwl-1 to bwh+1 do begin
             z:=bitrev[n];
             re0[n]:=realf[z];
             im0[n]:=imagf[z];
             z:=bitrev[f2l-n];
             re0[f2l-n]:=realf[z];
             im0[f2l-n]:=imagf[z];
             re1[n] := (re0[n] + re0[f2l-n]);
             im1[n] := (im0[n] - im0[f2l-n]);
             re2[n] := (im0[n] + im0[f2l-n]);
             im2[n] := (re0[f2l-n] - re0[n]);
             z:=n;
             IF abs(Re1[n])>abs(Im1[n]) THEN Mag[z]:=abs(Re1[n]) + abs(Im1[n])/2
             ELSE Mag[z]:=abs(Im1[n]) + abs(Re1[n])/2;
             z:=n+fftlen;
             IF abs(Re2[n])>abs(Im2[n]) THEN Mag[z]:=abs(Re2[n]) + abs(Im2[n])/2
             ELSE Mag[z]:=abs(Im2[n]) + abs(Re2[n])/2;
             END;
          END;


    BEGIN
       Init;
       repeat
          Inc(ctr);
          Samples;
          Calcfft;
          Spectra;
          FOR z:=bwl TO bwh DO Sig2sym;
          FOR z:=bwl+fftlen TO bwh+fftlen DO Sig2sym;
          until eof(inp);
       write('The End');
       readln;
       END.

