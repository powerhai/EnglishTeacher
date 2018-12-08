using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

//[JsonObject(IsReference = true)] 
public class Book{
    public int Id{get;set;}
    public string Title{get;set;}
    public virtual Publisher Publisher{get;set;}
    
    public  virtual ICollection<Text> Texts{get;set;} 
}