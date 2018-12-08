using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
 //[JsonObject(IsReference = true)] 
public class Publisher{
    public int Id{get;set;}
    public string Title{get;set;}

    public  virtual ICollection<Book> Books{get;set;}
}