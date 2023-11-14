using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace accesso_diretto
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public struct Prodotti//dichiarazione struct
        {
            public string nome;
            public float prezzo;
        }



        public Prodotti[] p = new Prodotti[100];//dichiarazione variabili
        public int dim=0 ;
        public int pos = 0; 
        public bool sin=false;
        public string filePath = "FILE.txt";
        public int riga = 64;

          
        public void leggi(string filePath)//funzione che sincronizza il file e la struct
        {
            string[] line = File.ReadAllLines(filePath);//vede quante righe ci sono scritte nel file
            int l = line.Length;
            sin= true;
            using (StreamReader read = File.OpenText(filePath))
            {
                string r = read.ReadLine();

                if (r != "")
                {

                    while (r != null)//assegna ogni riga a uno spazio nella struct
                    {
                        string[] prodotto = r.Split(';');
                        p[dim].nome = prodotto[0];
                        p[dim].prezzo = float.Parse(prodotto[1]);
                        dim++;

                        r = read.ReadLine();

                    }
                }

                read.Close();
            }
        }

        public void apriFile()//funzione che apre il file e permette di visualizzarlo
        {
            String files = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "File.txt");
            Process.Start(files);
        }

        public void Aggiunta(string nome, float prezzo, string filePath)//funzione che aggiunge un elemento al file
        {
            var apertura = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);//apre il file
            StreamWriter write = new StreamWriter(apertura);
            write.WriteLine($"{nome};{prezzo};0;".PadRight(riga - 4) + "##");//scrive una riga nel file con nome e prezzo indicati dall'utente
            write.Close();//chiude il file

        }

        public int Ricerca(string nome, string filePath, string prezzo)//funzione che ricerca un elemento nel file 
        {
            int posizione = 0;

            using (StreamReader read = File.OpenText(filePath))
            {
                string r = read.ReadLine();

                while (r != null)//scorre il file fino alla fine
                {
                    string[] prodotto = r.Split(';');

                    if (prodotto[0] == nome && prodotto[2] == "0" && prodotto[1]==prezzo)//se trova l'elemento prima della fine del file e restituisce la riga su cui si trova
                    {
                        return posizione;
                    }
                    r = read.ReadLine();
                    posizione++;
                }

                read.Close();
            }

                return -1;//se non trova l'elemento restituisce -1

        }

        public int RicercaProd( string filePath, string nome,string prezzo)//funzione che ricerca un elemento cancellato logicamente nel file
        {
            int posizione = 0;

            using (StreamReader read = File.OpenText(filePath))
            {
                string r = read.ReadLine();

                while (r != null)//scorre tutto il file
                {
                    string[] prodotto = r.Split(';');

                    if (prodotto[0] == nome && prodotto[2] == "1" && prodotto[1]==prezzo)//se trova l'elemento prima della fine del file e restituisce la riga su cui si trova
                    {
                        return posizione;
                    }
                    r = read.ReadLine();
                    posizione++;
                }

                read.Close();
            }
            return -1;//se non trova l'elemento restituisce -1

        }



        public void Modifica(int posizione, string nome, float prezzo, string filePath, int lunghezza)//funzione che modifica il prezzo e il nome di un elemento esistente
        {
            string nuovo;
            var file = new FileStream(filePath, FileMode.Open, FileAccess.Write);//apre il file
            BinaryWriter write = new BinaryWriter(file);
            file.Seek(lunghezza * posizione, SeekOrigin.Begin);//si posiziona sull'elemento da modificare
            nuovo = $"{nome};{prezzo};0;".PadRight(lunghezza - 4) + "##";//modifica l'elemento nel file
            p[posizione].nome = nome;//sostituisce i nuovi valori a quelli vecchi nella struct
            p[posizione].prezzo= prezzo;
            byte[] bytes = Encoding.UTF8.GetBytes(nuovo);
            write.Write(bytes);
            write.Close();
            file.Close();//chiude il file
        }

        public void Cancella(int posizione, string filePath, int lunghezza)//funzione che cancella logicamente gli elementi dal file
        {;
            string nuovo;
            var file = new FileStream(filePath, FileMode.Open, FileAccess.Write);//apre il file
            BinaryWriter write = new BinaryWriter(file);
            file.Seek(lunghezza * posizione, SeekOrigin.Begin);//si posiziona sull'elemento da cancellare
            nuovo = $"{p[posizione].nome};{p[posizione].prezzo};1;".PadRight(lunghezza - 4) + "##";//lo cancella impostando il 3° valore a 1
            byte[] bytes = Encoding.UTF8.GetBytes(nuovo);
            write.Write(bytes, 0, bytes.Length);
            write.Close();
            file.Close();//chiude il file
        }

        public void Rec( string filePath, int lunghezza, int posizione)//funzione che recupera gli elementi cancellati logicamente
        {

            string nuovo;
            var file = new FileStream(filePath, FileMode.Open, FileAccess.Write);//apre il file
            BinaryWriter write = new BinaryWriter(file);
            file.Seek(lunghezza * posizione, SeekOrigin.Begin);//si posiziona sull'elemento da recuperare
            nuovo = $"{p[posizione].nome};{p[posizione].prezzo};0;".PadRight(lunghezza - 4) + "##";//lo recuepra impostando il 3° valore a 0
            byte[] bytes = Encoding.UTF8.GetBytes(nuovo);
            write.Write(bytes, 0, bytes.Length);
            write.Close();
            file.Close();//chiude il file

        }

        public void Elimina(int posizione,string filePath,int lunghezza)//funzione che cancella fisicamente gli elementi dal file
        {
            DialogResult sicuro = MessageBox.Show("Sei sicuro di voler cancellare l'elemento?", "Cancella", MessageBoxButtons.YesNo, MessageBoxIcon.Question);// chiede all'utente se è sicuro di quello che vuola fare
            if (sicuro == DialogResult.Yes)
            {
                string[] line = File.ReadAllLines(filePath);//crea un array che contiene tutte le righe del file

                for (int i = posizione; i < line.Length - 1; i++)//riscrive l'array e la struct senza l'elemto da cancellare
                {
                    line[i] = line[i + 1];
                    p[i].nome = p[i + 1].nome;
                    p[i].prezzo = p[i + 1].prezzo;
                }

                var file = new FileStream(filePath, FileMode.Truncate, FileAccess.Write, FileShare.Read);//apre il file
                StreamWriter wr = new StreamWriter(file);
                wr.Write(string.Empty);//svuota il file
                wr.Close();//chiude il file

                var files = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read);//apre il file
                StreamWriter write = new StreamWriter(files);
                for (int i = 0; i < line.Length - 1; i++)//riscrive l'array nel file
                {
                    write.WriteLine(line[i]);
                }
                write.Close();//chiude il file
                dim--;
            }

        }

        public void Svuota()//funzione che cancella tutto quello che si trova nel file
        {
            DialogResult sicuro= MessageBox.Show("Sei sicuro di voler svuotare il file?", "Svuota File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);// chiede all'utente se è sicuro di quello che vuola fare
            if (sicuro == DialogResult.Yes)
            {
                var file = new FileStream(filePath, FileMode.Truncate, FileAccess.Write, FileShare.Read);//apre il file
                StreamWriter sw = new StreamWriter(file);
                sw.Write(string.Empty);//svuota il file
                sw.Close();//chiude il file
                for (int i = 0; i < dim; i++)//svuota la struct
                {
                    p[i].nome = "";
                    p[i].prezzo = 0;
                }
                dim = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)//pulsante che aggiunge un elemento al file
        {
            float c;
            if(sin==false)//controlla se file e struct sono sincronizzati
            {
                leggi(filePath);
            }
            if (textBox1.Text == "" || textBox2.Text == "")//controlla se tutti i campi sono stati compilati
            {
                MessageBox.Show("Errore: compila tutti i campi");
            }

            else
            {
                bool result = float.TryParse(textBox2.Text, out c);
                if (result)//controlla che il prezzo sia un numero
                {
                    if (float.Parse(textBox2.Text) >= 0)//controlla che il prezzo sia un numero positivo
                    {
                        p[dim].nome = textBox1.Text;//aggiunge un elemento alla struct
                        p[dim].prezzo = float.Parse(textBox2.Text);
                        Aggiunta(p[dim].nome, p[dim].prezzo, filePath);//aggiunge un elemento al file
                        dim++;
                        textBox1.Text = "";
                        textBox2.Text = "";
                        MessageBox.Show("Elemento aggiunto con successo");
                    }
                    else
                    {
                        MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                    }
                }
                else
                {
                    MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)// pulsante che cerca un elemento nel file e riporta la riga su cui si trova
        {
            float c;
            if (sin == false)// controlla che file e struct siano sincronizzate
            {
                leggi(filePath);
            }

            if (textBox1.Text == ""|| textBox2.Text=="")// controlla che tutti i campi siano stati compilati
            {
                MessageBox.Show("Errore: compila tutti i campi");
            }
            else
            {
                bool result = float.TryParse(textBox2.Text, out c);// controlla che il prezzo sia un numero
                if (result)
                {
                    if (float.Parse(textBox2.Text) >= 0)// controlla che il prezzo sia un numero positivo
                    {
                        pos = Ricerca(textBox1.Text, filePath, textBox2.Text);

                        if (pos == -1)// se l'elemento non viene trovato
                        {
                            MessageBox.Show("Il prodotto non è stato trovato");
                        }
                        else// se l'elemento viene trovato
                        {
                            MessageBox.Show("Il prodotto si trova sulla " + (pos + 1) + "° riga");
                        }
                        textBox1.Text = "";
                        textBox2.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                    }
                }
                else
                {
                    MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                }
            }
            
        }

        private void button3_Click(object sender, EventArgs e)// pulsante che modifica un elemento nel file
        {
            float c;
            if (sin == false)// controlla che file e struct siano sincronizzati
            {
                leggi(filePath);
            }
            if (textBox1.Text == "" ||textBox2.Text=="" || textBox3.Text == "" || textBox4.Text == "")// controlla che siano stati compilati tutti i campi
            {
                MessageBox.Show("Errore: compila tutti i campi");
            }
            else
            {
                bool result = float.TryParse(textBox4.Text, out c);// controlla che i prezzi siano dei numeri
                bool result1 = float.TryParse(textBox2.Text, out c);
                if (result && result1)
                {
                    if (float.Parse(textBox4.Text) >= 0 && float.Parse(textBox2.Text) >= 0)// controlla che i prezzi siano dei numeri positivi
                    {
                        pos = Ricerca(textBox1.Text, filePath, textBox2.Text);
                        if (pos == -1)// se l'elemento non è stato trovato
                        {
                            MessageBox.Show("Il prodotto non è stato trovato");
                        }
                        else// se l'elemento è stato trovato 
                        {
                            string nome = textBox3.Text;// modifica i valori dell'elemento
                            float prezzo = float.Parse(textBox4.Text);
                            Modifica(pos, nome, prezzo, filePath, riga);
                            MessageBox.Show("Elemento modificato con successo");
                        }
                        textBox1.Text = "";
                        textBox3.Text = "";
                        textBox4.Text = "";
                        textBox2.Text = "";
                    }

                    else
                    {
                        MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                    }
                }
                else
                {
                    MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)// pulsante che cancella logicamente un elemento dal file
        {
            float c;
            if (sin == false)// controlla che file e struct siano stati sincronizzati
            {
                leggi(filePath);
            }
            if (textBox1.Text == "" || textBox2.Text=="")// controlla che tutti i campi siano stati compilati
            {
                MessageBox.Show("Errore: compila tutti i campi");
            }
            else
            {
                bool result = float.TryParse(textBox2.Text, out c);// controlla che il prezzo sia un numero
                if (result)
                {
                    if (float.Parse(textBox2.Text) >= 0)// controlla che il prezzo sia un numero positivo
                    {
                        pos = Ricerca(textBox1.Text, filePath, textBox2.Text);

                        if (pos == -1)// se l'elemento non è stato trovato
                        {
                            MessageBox.Show("Il prodotto non è stato trovato");
                        }
                        else// se l'elemento è stato trovato
                        {
                            Cancella(pos, filePath, riga);// cancella logicamente l'elemento
                            MessageBox.Show("Elemento cancellato con successo");
                        }

                        textBox1.Text = "";
                        textBox2.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                    }
                }
                else
                {
                    MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                }
            }
            
        }

        private void button5_Click(object sender, EventArgs e)// pulsante che apre e permette di visualizzare il file
        {
            apriFile();
        }

        private void button6_Click(object sender, EventArgs e)// pulsante che recupera un elemento cancellato logicamente
        {
            float c;
            if (sin == false)// controlla che file e struct siano sincronizzati
            {
                leggi(filePath);
            }
            if (textBox1.Text == "" || textBox2.Text=="")// controlla che tutti i campi siano stati compilati
            {
                MessageBox.Show("Errore: compila tutti i campi");
            }
            else
            {
                bool result = float.TryParse(textBox2.Text, out c);// controlla che il prezzo sia un numero
                if (result)
                {
                    if (float.Parse(textBox2.Text) >= 0)// controlla che il prezzo sia un numero positivo
                    {
                        pos = RicercaProd(filePath, textBox1.Text, textBox2.Text);
                        if (pos == -1)// se l'elemento non è stato trovato
                        {
                            MessageBox.Show("Il prodotto non è stato trovato");
                        }
                        else// se l'elemento è stato trovato
                        {
                            Rec(filePath, riga, pos);// recupera l'elemento
                            MessageBox.Show("Elemento recuperato con successo");
                        }
                        textBox1.Text = "";
                        textBox2.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                    }
                }
                else
                {
                    MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)// pulsante che cancella fisicamente un elemento dal file
        {
            float c;
            if (sin == false)// controlla che file e struct siano sincronizzati
            {
                leggi(filePath);
            }
            if (textBox1.Text == "" || textBox2.Text=="")// controlla che tutti i campi siano stati compilati
            {
                MessageBox.Show("Errore: compila tutti i campi");
            }
            else
            {
                bool result = float.TryParse(textBox2.Text, out c);// controlla che il prezzo sia un numero
                if (result)
                {
                    if (float.Parse(textBox2.Text) >= 0)// controlla che il prezzo sia un numero positivo
                    {
                        pos = Ricerca(textBox1.Text, filePath, textBox2.Text);
                        if (pos == -1)// se l'elemento non è stato trovato
                        {
                            MessageBox.Show("Il prodotto non è stato trovato");
                        }
                        else// se l'elemento è stato trovato
                        {
                            Elimina(pos, filePath, riga);// cancella fisicamente l'elemento
                            MessageBox.Show("Elemento eliminato con successo");
                        }

                        textBox1.Text = "";
                        textBox2.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                    }
                }
                else
                {
                    MessageBox.Show("Errore: i campi non sono stati compilati correttamente");
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)// pulsante che chiude il programma
        {
            DialogResult sicuro = MessageBox.Show("Sei sicuro di voler uscire dal programma?", "Esci", MessageBoxButtons.YesNo, MessageBoxIcon.Question);// chiede all'utente se è sicuro di quello che vuola fare
            if (sicuro == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button9_Click(object sender, EventArgs e)// pulsante che svuota il file
        {
            Svuota();
        }
    }
}
