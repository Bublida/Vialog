using System.Text;

namespace Vialog
{

public class Dialog
{
    /// <summary>List of the dialog`s frases</summary>
    public List<Frase> Frases;
    /// <summary>The active frase at dialog</summary>
    public Frase ActiveFrase { get; private set; }

    private Dialog(){ 
        Frases = []; 
    }

    /// <summary>creates a empty dialog</summary>
    /// <returns>Empty dialog object</returns>
    public static Dialog CreateEmpty(){ 
        return new Dialog(); 
    }

    /// <summary>creates a dialog with a basic phrase and basic answers</summary>
    /// <returns>Dialog object</returns>
    public static Dialog CreateTemplate(){
        var template = new Dialog();
        template.Frases.Add( new("Foofel","Hello world!",[new("Hi!"),new("Bye bye")]) );
        return template;
    }

    /// <summary>creates a dialog from a file with the .vialog extension</summary>
    /// <param name="path">File path</param>
    /// <returns>Dialog object</returns>
    public static Dialog OpenFromFile(string path){
        var dialog = new Dialog();
        using(StreamReader f = new(path, Encoding.UTF8)) {
            List<Answer>? answers = [];
            string name = "", text = "";
            do{
                string? line = f.ReadLine();
                if(line!=null && line.Length>0){
                    switch(line.First()){
                        case '<': name = line[1..^1]; break;
                        case '?': answers.Add(new(line[1..])); break;
                        case '-': text = line[1..]; break;
                    }
                }else{ 
                    dialog.Frases.Add(new(name, text, answers)); 
                    answers = []; 
                }
            }while(!f.EndOfStream);
            if(f.EndOfStream){ dialog.Frases.Add(new(name, text, answers)); }
        }
        dialog.ActiveFrase = dialog.Frases.First();
        return dialog;
    }

    /// <summary></summary>
    /// <param name="path"></param>
    public void Save(string path){
        using(StreamWriter w = new(path, false, Encoding.UTF8)){
            string name = Frases.First().Speaker;
            w.WriteLine($"<{name}>");
            for(int i = 0; i < Frases.Count; i++){
                Frase frase = Frases[i];
                if(frase.Speaker!=name){
                    name = frase.Speaker;
                    w.WriteLine($"<{name}>");
                    continue;
                }
                w.WriteLine('-'+frase.Text);
                foreach(var answer in frase.Answers){
                    w.WriteLine('?'+answer.Text);
                }
                if(i!=Frases.Count-1) w.WriteLine();
            }
        }
    }

    /// <summary>ыwitching to the next phrase</summary>
    public void NextFrase(){
        int ind = Frases.IndexOf(ActiveFrase);
        if(ind!=Frases.Count-1) ActiveFrase = Frases[Frases.IndexOf(ActiveFrase)+1];
    }
    /// <summary></summary>
    /// <param name="name"></param>
    public void NextFrase(string name){
        ActiveFrase = Frases.Find(f => f.Speaker==name);
    }

    /// <summary></summary>
    public void PreviousFrase(){
        int ind = Frases.IndexOf(ActiveFrase);
        if(ind!=0) ActiveFrase = Frases[Frases.IndexOf(ActiveFrase)-1];
    }

    /// <summary></summary>
    /// <param name="index"></param>
    public void SetFrase(int index){
        ActiveFrase = Frases[index];
    }

    /// <summary>console debug output</summary>
    public void Show(){
        foreach(var frase in Frases){
            Console.WriteLine(frase.Speaker+"> "+frase.Text);
            for(int i = 0; i < frase.Answers.Count; i++){
                Console.WriteLine($"\t{i+1}) {frase.Answers[i].Text}");
            }
        }
    }
}

public struct Frase(string name, string text, List<Answer> answers)
{
    /// <summary>Name of the speaker of the phrase</summary>
    public string Speaker = name;
    /// <summary>Text of the frase</summary>
    public string Text = text;
    public List<Answer>? Answers = answers;

    /// <summary>console debug output</summary>
    public readonly void Show(){
        Console.WriteLine(Speaker + "> " + Text);
        for (int i = 0; i < Answers.Count; i++){
            Console.WriteLine($"\t{i + 1}) {Answers[i].Text}");
        }
    }
}

public struct Answer(string answer)
{
    public string Text = answer;
}

}