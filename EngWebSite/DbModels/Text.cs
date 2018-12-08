using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

//[JsonObject(IsReference = true)] 
public class Text{
    public int Id{get;set;}
    public string Title{get;set;}
    
    public string Body{get;set;}
    public virtual Book Book{get;set;}
    public  virtual ICollection<Sentence> Sentences{get;set;} 
}