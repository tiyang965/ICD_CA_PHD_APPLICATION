# Short explanation

*Stuttgart, 2022.12.08*

*Tianjiao Yang*

-------------------------------------------------------------------

The uploaded python and c# codes implement named pipes for exchanging double arrays between a C# and a Python process. 

Both the Client and the Server are written in the same python file (*ndpp.py*), selective with an input argument s (for Server) or c (for Client), while the Server and the Client are written seperately in the c# files (*npS.cs* & *npC.cs*, respectively).

To test the codes, random 2d arrays with float numbers are generated in the Server, meaning **line 10** in *ndpp.py* and **line 24** in *npS.cs*. 

The Client prints out the received data, line by line, to the console during the transmission process. After transmission is complete, the entire array is printed to the console once again for verification.

