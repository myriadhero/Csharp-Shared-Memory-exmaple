/*
This is C# p2 program for shared memory client

 */


using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace console
{
    class Program
    {
        static void Main(string[] args){
            Console.WriteLine("Opening shared memory...\n\n");

            try{
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("Global\\ServerMemoryP1")){

                    /* // this block was used to read entire string
                    using (MemoryMappedViewStream stream = mmf.CreateViewStream()){
                        //BinaryReader reader = new BinaryReader(stream);
                        StreamReader reader = new StreamReader(stream);
                        Console.Write("Server p1 says:"); //reader.ReadBoolean()
                        Console.WriteLine("{0}",reader.Read());
                    }
                    */

                    // is found
                    // need to open the view and read the value

                    using(var serverP1Message = mmf.CreateViewAccessor()){ // access the view in the shared memory
                        double messageFromP1 = serverP1Message.ReadDouble(0);   // there are many .ReadX commands available
                        Console.WriteLine("ServerP1 says: {0:N}", messageFromP1);   
                        
                        while(true){ // just waiting until the program is either terminated or a new value is entered
                            int terminatorWaitCond = 0;

                            while(serverP1Message.ReadDouble(0) == messageFromP1){ //wait for changes in shared memory and check every 200ms
                                System.Threading.Thread.Sleep(200);
                                terminatorWaitCond ++;

                                if(terminatorWaitCond >= 150) { // terminate the program after 30s
                                    Console.WriteLine("\n serverP1 hasn't sent a new value in a while, terminating the program...");
                                    Console.Read();
                                    return;
                                }
                            }
                            
                            messageFromP1 = serverP1Message.ReadDouble(0);
                            Console.WriteLine("ServerP1 says: {0:N}", messageFromP1);
                            if(messageFromP1 == 10){
                                Console.WriteLine("ServerP1 says: terminate");
                                Console.Read();
                                return;
                            }
                        }
                    }
                    
                }
            }
            catch (FileNotFoundException){
                Console.WriteLine("Memory-mapped file does not exist. Run server p1 first, then restart this program.");
            }



            // Console.WriteLine("\n\n type 0 to terminate the program: ");
            Console.Read();

        }


    


    }
}

