public class Sentence{
    public int Id{get;set;}
    public string English{get;set;}
    public string Chinese{get;set;}
      
    public byte[] Audio{get;set;}
    public string FileName{get;set;}
    public virtual Text Text {get;set;}
}