using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;
using System.Media;

namespace Voice_Bot
{
    public partial class Form1 : Form
    {
        //On/off
        Boolean on = false;

        //Grammar and Reponses
        String[] grammarFile = (File.ReadAllLines(@"C:\Voice Bot\grammar.txt"));
        String[] responseFile = (File.ReadAllLines(@"C:\Voice Bot\response.txt"));

        //Speech Synth
        SpeechSynthesizer speechSynth = new SpeechSynthesizer();

        //Speech Rec
        Choices grammarList = new Choices();
        SpeechRecognitionEngine speechRecognition = new SpeechRecognitionEngine();

        //my classes
        String[] classesFile = (File.ReadAllLines(@"C:\Voice Bot\classes.txt"));
        String[] zoomFile = (File.ReadAllLines(@"C:\Voice Bot\zoom.txt"));

        public Form1()
        {
            this.WindowState = FormWindowState.Minimized;


            //Init Grammar
            grammarList.Add(grammarFile);
            Grammar grammar = new Grammar(new GrammarBuilder(grammarList));

            try
            {
                //RequestRecognizerUpdate updates
                speechRecognition.RequestRecognizerUpdate();
                speechRecognition.LoadGrammar(grammar);
                //SpeechReconized raises when receives input that matches
                speechRecognition.SpeechRecognized += rec_SpeechRecognized; 
                speechRecognition.SetInputToDefaultAudioDevice(); // uses my audio device
                speechRecognition.RecognizeAsync(RecognizeMode.Multiple); // say smth, it'll reset
            }
            catch {
                
                return;
            }


            //Custon Speech Synth Settings
            speechSynth.SelectVoiceByHints(VoiceGender.Female);


            InitializeComponent();
        }

        public void restart()
        {
            Application.Restart();
        }

        public void say(String text)
        {
            
            speechSynth.SpeakAsync(text);
            outputTextBox.AppendText(text + "\n");
            on = false;
        }


        private void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String result = e.Result.Text;
            int resp = Array.IndexOf(grammarFile, result); //find line number



            if(responseFile[resp].IndexOf('*') == 0) //complex commands
            {
                if (result.Contains("hey")) //exit the program
                {
                    on = true;
                    SystemSounds.Beep.Play();
                }
                

                if (Process.GetProcessesByName("Discord").Length > 0 && result.Contains("deafen") && on) //deaf
                {
                    SendKeys.Send("\\");
                    on = false;
                    say(result);
                }
                
                if (Process.GetProcessesByName("Discord").Length > 0 && result.Contains("mute") && on) //mute
                {
                    SendKeys.Send("{INSERT}");
                    on = false;
                    say(result);
                }

                if ((result.Contains("exit") || result.Contains("quit")) && on) //exit the program
                {
                    this.Close();
                    return;
                }

                if (result.Contains("shut down") && on && state.Text.Contains("On")) //turn off
                {
                    say("good bye");
                    on = false;
                    state.Text = "State: Off";
                }
                if (result.Contains("activate") && state.Text.Contains("Off") && on) //activate
                {
                    say("I'm online");
                    on = true;
                    state.Text = "State: On";
                }
                if (result.Contains("restart")) //reboot
                {
                    say("rebooting");

                    System.Threading.Thread.Sleep(1000); //let's the program say rebooting

                    restart();
                }
                if (result.Contains("nevermind") && on && state.Text.Contains("On")) //activate
                {
                    say("Okay");
                    on = false;
                    
                }

                if (result.Contains("print screen") && on && state.Text.Contains("On")) //print screen
                {
                    
                    SendKeys.Send("^%{PRTSC}");
                    on = false;
                    say(result);
                }

                if (Process.GetProcessesByName("DMT").Length > 0 && result.Contains("lock screen") && on && state.Text.Contains("On"))
                {
                    SendKeys.Send("+{DOWN}");
                    on = false;
                    say("locked");
                } else if (result.Contains("lock screen") && on && state.Text.Contains("On"))
                {
                    Process.Start(@"C:\Users\Osbert\Desktop\dmt\DMT.exe");
                    System.Threading.Thread.Sleep(2000);
                    SendKeys.Send("+{DOWN}");
                    on = false;
                    say("locked");
                }
                
                if (result.Contains("snipping tool") && on && state.Text.Contains("On")) // snipping
                {
                    SendKeys.Send("^%{UP}");
                    on = false;
                    say(result);
                }
                if (result.Contains("minimize") && on && state.Text.Contains("On")) //minimize
                {
                    this.WindowState = FormWindowState.Minimized;
                    on = false;
                    say("hiding");
                }
                if (result.Contains("come back") && on && state.Text.Contains("On")) //normal
                {
                    this.WindowState = FormWindowState.Normal;
                    on = false;
                    say("I'm back");
                }
                if (result.Contains("who") && on && state.Text.Contains("On")) //give date
                {
                    say("I was created by this asian male");
                    Process.Start("http://linkedin.com/in/osblee/");

                }
                if (result.Contains("favorites") && on && state.Text.Contains("On")) //opens favorites
                {
                    Process.Start("http://youtube.com/");
                    System.Threading.Thread.Sleep(500);
                    Process.Start("http://kissanime.ru/");
                    System.Threading.Thread.Sleep(500);
                    Process.Start("http://kissmanga.com/");
                    on = false;
                    say("launching");
                }
                if (result.Contains("school") && on && state.Text.Contains("On")) //opens school
                {
                    Process.Start("https://courses.cs.washington.edu/courses/cse143/20sp/");
                    System.Threading.Thread.Sleep(500);
                    Process.Start("https://canvas.uw.edu/");
                    
                    on = false;
                    say("launching");
                }
                if ((result.Contains("when is class") || result.Contains("go to class")) && on && state.Text.Contains("On")) //go to class
                { // file should be formatted like so
                  // Monday
                  // (amount of classes)
                  // 12 30 PM CSE LECTURE

                    if (classesFile.Contains(DateTime.Now.ToString("dddd")))
                    {
                        int iDay = 0;
                        int classNum = 0;
                        for (int i = 0; i < classesFile.Length; i++)
                        {
                            if (DateTime.Now.ToString("dddd").Equals(classesFile[i]))
                            {
                                iDay = i;
                                classNum = Int16.Parse(classesFile[i + 1]);
                            }
                        }

                        string myClass = "";
                        int iClass = iDay + 2;
                        for (int i = 0; i < classNum; i++)
                        {
                            String[] classInfo = classesFile[iClass + i].Split(' ');

                            int currentHour = DateTime.Now.Hour;
                            int classHour = Int16.Parse(classInfo[0]);

                            if (classInfo[2].Equals("PM") && classHour != 12)
                            {
                                classHour += 12;
                            }
                            //say("" + currentHour + " " + classHour + " " + DateTime.Now.Minute + " " + Int16.Parse(classInfo[1]));

                            if (currentHour == classHour && DateTime.Now.Minute <= Int16.Parse(classInfo[1]))
                            {
                                say("Class starts in: " + (Int16.Parse(classInfo[1]) - DateTime.Now.Minute) + " minutes");
                                myClass = classInfo[3] + " " + classInfo[4];
                                break;
                            }
                            else if ((classHour - currentHour == 1) && ((60 - DateTime.Now.Minute) + Int16.Parse(classInfo[1]) < 60))
                            {
                                say("Class starts in: " + ((60 - DateTime.Now.Minute) + Int16.Parse(classInfo[1])) + " minutes");
                                myClass = classInfo[3] + " " + classInfo[4];
                                break;
                            }
                            else if ((currentHour < classHour) && (DateTime.Now.Minute > Int16.Parse(classInfo[1])))
                            {
                                say("Class starts in: " + (classHour - currentHour - 1) + " hours and " +
                                        ((60 - DateTime.Now.Minute) + Int16.Parse(classInfo[1])) + " minutes");
                                myClass = classInfo[3] + " " + classInfo[4];
                                break;

                            }
                            else if ((currentHour < classHour) && (DateTime.Now.Minute < Int16.Parse(classInfo[1])))
                            {
                                say("Class starts in: " + (classHour - currentHour) + " hours and " +
                                        (Int16.Parse(classInfo[1]) - DateTime.Now.Minute) + " minutes");
                                myClass = classInfo[3] + " " + classInfo[4];
                                break;
                            }
                        }

                        if (myClass.Equals(""))
                        {
                            say("Done with classes today");
                        }
                        else
                        {
                            say(myClass);
                            if (result.Contains("go to class"))
                            {

                                for (int i = 0; i < zoomFile.Length - 1; i++)
                                {
                                    if (myClass.Equals(zoomFile[i]))
                                    {
                                        say("Launching zoom");
                                        Process.Start(zoomFile[i + 1]);
                                        break;
                                    }
                                }
                            }
                        }


                        
                    }
                    else
                    {
                        say("No class today");
                    }

                    on = false;

                }



                if (result.Contains("time") && on && state.Text.Contains("On")) //give time
                {
                    say(DateTime.Now.ToString(@"hh\:mm tt"));
                }
                if (result.Contains("date") && on && state.Text.Contains("On")) //give date
                {
                    say(DateTime.Now.ToString("dddd MMMM dd yyyy"));
                }
                
            }
            inputTextBox.AppendText(result + "\n");

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
