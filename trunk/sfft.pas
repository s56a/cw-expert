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
       r: word;
       b: byte;
       ch: CHAR;
       key: BOOLEAN;
       s,morse: string[63];
       output: array [0..f2l] of STRING;
       sig,sum,ave: array [0..f2l] of Integer;
       RealF, ImagF, Mag, prag: ARRAY [0..f2l] OF Real;
       broj: array [0..f2l] of byte;
       si, co, wd: ARRAY [0..f2l] OF Real;
       tim: array [0..f2l] of longint;
       v, thd, Period: Real;
       t,ctr: longint;
       inp: file of word;
       log: text;
       dup,cls: array [0..220] of string[6];
       okc, clc, j, n, n1, w, x, y, z, l, loopend, totalsamples, dotmin: INTEGER;
       bitrev, dma1, old1, dma2, old2: array [0..f2l] of byte;
       keyes: array [0..f2l] of boolean;


    PROCEDURE Init;

       BEGIN
          assign(inp,'kc21.wav');
          reset(inp);
          for n:=1 to 29 do read(inp,r);
          ctr:=0;
          okc:=0;
          clc:=0;
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
          writeln(clc-1);
          TotalSamples:=f2l;
          l:=Round(Ln(TotalSamples)/Ln(2));
          Period:=TotalSamples/rate;
          FOR n:=0 TO TotalSamples-1 DO BEGIN
             x:=n;
             Y:=0;
             N1:=TotalSamples;
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
          dotmin:=round(0.03/period);  {40 wpm, 30 msec dot}
          morse:='ETIANMSURWDKGOHVF*L*PJBXCYZQ**54*3***2 ******16*/*****7***8*90*';
          for n:=0 to f2l do begin
             prag[n]:=noise;
             ave[n]:=aver;
             sum[n]:=0;
             tim[n]:=1;
             old1[n]:=0;
             dma1[n]:=0;
             dma2[n]:=0;
             old2[n]:=0;
             keyes[n]:=false;
             output[n]:='';
             broj[n]:=0;
             end;
          v:=2*Pi/TotalSamples;
          FOR n:=0 TO TotalSamples-1 DO BEGIN
             RealF[n]:=0;
             ImagF[n]:=0;
             si[n]:=-Sin(n*v);
             co[n]:= Cos(n*v);
             wd[n]:=(0.54+0.46*co[n])/FFTlen;  {Hamming}
             END;
          END;


    Procedure cw2asc;

       BEGIN
          IF sum[z]<63 THEN ch:=morse[sum[z]]
          ELSE IF sum[z]=75 THEN ch:='?'
          ELSE ch:='*';
          output[z]:=output[z]+ch;
          sum[z]:=0;
          END;


    Procedure Filter;

       var loc: integer;
           prn: boolean;

       begin
          IF s[1] IN ['0'..'9'] THEN loc:=3 else loc:=1;
          repeat
             prn:=s[loc] in ['0'..'9'];
             inc(loc);
             until prn or (loc>length(s));
          loc:=0;
          if clc>0 then repeat
             prn:=s=cls[loc];
             Inc(loc);
             until prn or (loc>clc);
          if prn then BEGIN
             n:=0;
             while (n<okc) and (s<>dup[n]) do inc(n);
             if (s<>dup[n]) AND (clc>0) then begin
                writeln(n+1:3,ctr:6,z:6,round(prag[z]):6,ave[z]:6,'  ',s);
                dup[n]:=s;
                inc(okc);
                end;
             end;
          end;


    Procedure CWdecode;

       begin
          keyes[z]:=key;
          t:=ctr-tim[z];
          tim[z]:=ctr;
          if key then begin
             if t>ave[z] then begin
                IF sum[z]>0 THEN cw2asc
                end
             else begin
                sum[z]:=2*sum[z];
                if sum[z]>255 then sum[z]:=0;
                if t>dotmin then ave[z]:=t + (ave[z] div 2);
                end
             end
          else begin
             if t>ave[z] then inc(sum[z])
             else if t>dotmin then ave[z]:=t + (ave[z] div 2);
             Inc(sum[z]);
             end;
          end;


     procedure counters;
          begin
             if (ctr-tim[z])=(2*ave[z]) then begin
                if sum[z]>0 then cw2asc;
                s:=output[z];
                if (length(s)>3) then Filter;
                output[z]:='';
                ave[z]:=aver;
                end;
             end;


    PROCEDURE Sig2Sym;

       BEGIN
             v:=mag[z];
             { for j:=-1 to 1 do v:=v + mag[z+j];  }
             thd:=(31*prag[z]+v)/32;
             prag[z]:=thd;
             thd:=(thd+noise)/2;
             if thd<noise then thd:=noise;
             b:=broj[z];
             if v>thd then begin
                if b<3 then inc(b)
                end
             else if b>0 then dec(b);
             broj[z]:=b;
             key:=b>1;
             if key<>keyes[z] then CWDecode
             else if not key then Counters;
          end;


    PROCEDURE CallFFT;

       VAR
          i,k,m,mx,I1,I2,I3,I4,I5: INTEGER;
          A1,A2,B1,B2,Z1,Z2: Real;

       BEGIN
          I1:=TotalSamples div 2;
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


    PROCEDURE DoIt;

       BEGIN
          FOR n:=0 TO FFTlen-1 DO BEGIN
             if eof(inp) then r:=128+256*128
             else read(inp,r);
             dma1[n]:=r div 256;
             dma2[n]:=r mod 256;
             ImagF[n]:=0;
             ImagF[n+FFTlen]:=0;
             RealF[n+FFTlen]:=wd[n+FFTlen]*old1[n];
             old1[n]:=dma1[n];
             RealF[n]:=wd[n]*old1[n];
             end;
          Callfft;
          for z:=bwl-1 to bwh+1 do begin
             y:=BitRev[z];
             IF abs(RealF[y])>abs(ImagF[y]) THEN Mag[z]:=abs(RealF[y]) + abs(ImagF[y])/2
             ELSE Mag[z]:=abs(ImagF[y]) + abs(RealF[y])/2;
             END;
          for z:=bwl to bwh do Sig2sym;
          FOR n:=0 TO FFTlen-1 DO BEGIN
             ImagF[n]:=0;
             ImagF[n+FFTlen]:=0;
             RealF[n+FFTlen]:=wd[n+FFTlen]*old2[n];
             old2[n]:=dma2[n];
             RealF[n]:=wd[n]*old2[n];
             end;
          CallFFT;
          for z:=bwl-1 to bwh+1 do begin
             y:=BitRev[z];
             IF abs(RealF[y])>abs(ImagF[y]) THEN Mag[z+fftlen]:=abs(RealF[y]) + abs(ImagF[y])/2
             ELSE Mag[z+fftlen]:=abs(ImagF[y]) + abs(RealF[y])/2;
             END;
          for z:=bwl+fftlen to bwh+fftlen do sig2sym;
          END;


    BEGIN
       Init;
       repeat
          Inc(ctr);
          DoIt;
          until eof(inp);
       write(' The End');
       readln;
       END.

