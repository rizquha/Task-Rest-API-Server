﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System.Text;
using System.Linq;

namespace RestAPI
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootApp = new CommandLineApplication()
            {
                Name = "Aplikasi List",
                Description = "Digunakan untuk membuat list yang harus dilakukan",
                ShortVersionGetter = () => "1.0.0",
                
            };

            rootApp.Command("todo",app =>
            {
                app.Description="List Kegiatan";
                var list = app.Option("--list","show list",CommandOptionType.NoValue);
                var clear = app.Option("--clear","clear list",CommandOptionType.NoValue);
                var add = app.Option("--add","add list",CommandOptionType.MultipleValue);
                var update = app.Option("--update","update list",CommandOptionType.MultipleValue);
                var delete = app.Option("--delete","delete list",CommandOptionType.SingleOrNoValue);
                var done = app.Option("--done","done list",CommandOptionType.SingleOrNoValue);

                app.OnExecuteAsync(async cancellationToken => 
                {
                    HttpClientHandler clientHandler = new HttpClientHandler();
                    clientHandler.ServerCertificateCustomValidationCallback = (sender,cert,chain,sslPolicyErrors)=>
                    {
                        return true;
                    };
                    HttpClient client = new HttpClient(clientHandler);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,"https://localhost:5001/list");

                    if(list.HasValue())
                    {
                        HttpResponseMessage response = await client.SendAsync(request);
                        var json = await response.Content.ReadAsStringAsync();

                        var obj = JsonConvert.DeserializeObject<List<todo>>(json);

                        Console.WriteLine("List Kegiatan Yang Harus Dilakukan : ");
                        int counter =1;
                        foreach(var list in obj)
                        {
                            if(list.status==true)
                            {
                                Console.WriteLine(counter+". "+list.list+" = Done");
                            }
                            else
                            {
                                Console.WriteLine(counter+". "+list.list+" = Undone");  
                            }
                            counter++;
                        }
                    }
                    
                    if(clear.HasValue())
                    {
                        HttpResponseMessage response = await client.SendAsync(request);
                        var json = await response.Content.ReadAsStringAsync();

                        var obj = JsonConvert.DeserializeObject<List<todo>>(json);
                        var list = from item in obj
                                   select new {id=item.id,list=item.list};

                        var sure = Prompt.GetYesNo("Sure ?",false);
                        if(sure)
                        {
                            foreach(var item in list)
                            {
                                await client.DeleteAsync("https://localhost:5001/list/"+item.id);
                            }
                        }
                    }

                    if(add.HasValue())
                    {
                        var todo = new todo()
                        {
                            id=Convert.ToInt32(add.Values[0]),
                            list=add.Values[1]
                        };
                        var json= JsonConvert.SerializeObject(todo);
                        var content = new StringContent(json,Encoding.UTF8,"application/json");
                        await client.PostAsync("https://localhost:5001/list",content);

                    }
                    
                    if(update.HasValue())
                    {
                        var todo =  "{"
                                    +$"\"list\":\"{update.Values[1]}\""
                                    +"}";
                        var id = Convert.ToInt32(update.Values[0]);
                        var content = new StringContent(todo,Encoding.UTF8,"application/json");
                        await client.PatchAsync("https://localhost:5001/list/"+ id,content);
                    }

                    if(delete.HasValue())
                    {
                        var id = delete.Value();
                        await client.DeleteAsync("https://localhost:5001/list/"+Convert.ToInt32(id));
                    }
                    
                    if(done.HasValue())
                    {
                        var todo ="{"
                                 +"\"status\":"
                                 +"true"
                                 +"}";
                        var content = new StringContent(todo,Encoding.UTF8,"application/json");
                        var id = Convert.ToInt32(done.Value());
                        await client.PatchAsync("https://localhost:5001/list/status/"+id,content);
                    }
                });
            });
            
            rootApp.OnExecute(()=>
            {
                rootApp.ShowHelp();
            });
            return rootApp.Execute(args);
        }
    }
    class todo
    {
        [JsonProperty("id")]
        public int id{get;set;}
        [JsonProperty("list")]
        public string list {get;set;}
        [JsonProperty("status")]
        public bool status {get;set;}=false;

    }
}
