using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;


namespace Awesome
{
    class Program
    {
        static ITelegramBotClient botClient;
        static bool validabots = true;
        static string mensaje_de_bienvenida;

        static void Main()
        {
            botClient = new TelegramBotClient("873749474:AAFY4D7g8DgyJYBdxeXeq0UENFWvRkoek_k");//878571487:AAF_5ome_5mGeEz1xKdc9e0BvmPCqduajgs");//873749474:AAFY4D7g8DgyJYBdxeXeq0UENFWvRkoek_k");

            var me = botClient.GetMeAsync().Result;
            //verifica que el bot ande y da el id y nombre
            Console.WriteLine(
              $"Hola Mundo! Soy {me.Id} y mi nombre es  {me.FirstName}."
            );
            mensaje_de_bienvenida = "Te damos la bienvenida al chat de Geek Out! Argentina, este es un grupo donde hablamos principalmente de juegos de mesa, comics, películas, series y otras nerdeadas. ¿Te gusta alguna de estas cosas?\n ";
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();
          Thread.Sleep(int.MaxValue);
        }

        //static void validabots() { }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            string accion;

            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                

                if (!e.Message.Text.StartsWith("/"))
                {
                    accion = "mensaje";
                }
                else
                {
                    accion = e.Message.Text.Split(' ').First();
                    Console.WriteLine("accion es:" + accion);

                    //Console.WriteLine("Nombre del bot es:" + botClient.bot);

                    bool esadmin = true;
                    string status_usuario = botClient.GetChatMemberAsync(e.Message.Chat.Id, e.Message.From.Id).Result.Status.ToString();
                    Console.WriteLine($"Tipo de mensaje de usuario es " + botClient.GetChatMemberAsync(e.Message.Chat.Id, e.Message.From.Id).Status);
                    Console.WriteLine($"usuario es " + botClient.GetChatMemberAsync(e.Message.Chat.Id, e.Message.From.Id).Result.Status);
                    //Console.WriteLine($"usuario es " + botClient.GetChatMemberAsync(e.Message.Chat.Id, e.Message.From.Id).Id);
                    if (status_usuario.ToLower()  == "administrator"
                         || status_usuario.ToLower() == "creator")

                    { esadmin = true; }
                    else { esadmin = false; }

                    switch (accion)
                    {
                        case "/validabots":
                            accion = "validabots";

                            //String  tipo_de_miembro = botClient.GetChatMemberAsync(e.Message.Chat.Id , e.Message.From.Id).Result.Status ;
                            //if (tipo_de_miembro == "administrator")
                            //{
                            //    tienepermiso = true;
                            //}


                            if (esadmin)
                            {
                                validabots = !validabots;
                                if (validabots)
                                {
                                    await botClient.SendTextMessageAsync(
                                                chatId: e.Message.Chat,
                                                text: "No se permiten bots"
                                                );
                                }
                                else
                                {
                                    await botClient.SendTextMessageAsync(
                                                chatId: e.Message.Chat,
                                                text: "Si se permiten bots"
                                                );
                                }
                            }
                            else
                            {
                                no_tiene_permiso(e);
                            }

                            break;
                        case "/bienvenida":
                            if (esadmin)
                            {
                                int i = e.Message.Text.IndexOf(" ") + 1;
                                mensaje_de_bienvenida = e.Message.Text.Substring(i);
                                await botClient.SendTextMessageAsync(
                                              chatId: e.Message.Chat,
                                              text: "Se cambio mensaje de bienvenida"
                                              );
                            }
                            else
                            {
                                no_tiene_permiso(e);
                            }
                            
                            break;
                        case "/getoutofjail":
                            if (esadmin)
                            {
                                int i = e.Message.Text.IndexOf(" ") + 1;
                                int id_persona;
                                string persona_liberada= e.Message.Text.Substring(i);
                                Console.WriteLine();
                                if (e.Message.ReplyToMessage.From.Id ==null)
                                {
                                    await botClient.SendTextMessageAsync(
                                              chatId: e.Message.Chat,
                                              text: "Tiene que responder a un mensaje de la persona que desea desbloquear."
                                              );
                                }
                                else
                                {
                                    botClient.RestrictChatMemberAsync(e.Message.Chat.Id, e.Message.ReplyToMessage.From.Id, DateTime.Now, true, true, true, true);
                                }                                
                            }
                            else
                            {
                                no_tiene_permiso(e);
                            }
                            break;
                        default: 
                            accion = "mensaje";
                            break;

                    }
                }
                
            }

            //si entra alguien nuevo
            if (e.Message.NewChatMembers != null)
            {
                //si es bot comun, no es el mismo bot y esta activado validabot lo patea
                if (e.Message.NewChatMembers[0].IsBot && e.Message.NewChatMembers[0].Id != botClient.BotId &&  validabots )
                {
                    botClient.KickChatMemberAsync(e.Message.Chat.Id, e.Message.NewChatMembers[0].Id);
                    await botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: textorandom()
                        );
                }
                else
                {//si no es bot, le da la bienvenida

                    //*** comments para debugear ***
                    //Console.WriteLine($"Se unio {e.Message.NewChatMembers[0].Id }.");
                    //await botClient.SendTextMessageAsync(
                    //  chatId: e.Message.Chat,
                    //  text: "Se unio alguien con ID= " + e.Message.NewChatMembers[0].Id + "\n Al chatId: " + e.Message.Chat.Id + "\n"
                    //);
                    //*** fin comments ***

                    //parche para que no de bienvenida a los nuevos usuarios al prenderlo cada mañana.
                    //if (botClient.GetChatMemberAsync(e.Message.Chat.Id,e.Message.NewChatMembers[0].Id).Status.ToString()  =="member")
                    //{

                    //}
                    if (e.Message.NewChatMembers[0].Username == null)
                    {
                        await botClient.SendTextMessageAsync(
                          chatId: e.Message.Chat,
                          text: "Hola "+ e.Message.NewChatMembers[0].FirstName + " " + e.Message.NewChatMembers[0].LastName + mensaje_de_bienvenida 
                      );
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                              chatId: e.Message.Chat,
                              text: "Hola " + e.Message.NewChatMembers[0].Username + mensaje_de_bienvenida 
                          );
                    }

                    await botClient.SendTextMessageAsync(
                              chatId: e.Message.Chat,
                              text: "Se restringio acceso para " + e.Message.NewChatMembers[0].FirstName + " " + e.Message.NewChatMembers[0].LastName
                          );
                    //Setea los permisos
                    botClient.RestrictChatMemberAsync(e.Message.Chat.Id, e.Message.NewChatMembers[0].Id, DateTime.Now.AddDays(7), true, false, false, false);

                }
            }
            else
            {
                Console.WriteLine($"No se unio nadie");
            }

            
            

        }

        static void no_tiene_permiso(MessageEventArgs e)
        {
            botClient.SendTextMessageAsync(
                                              chatId: e.Message.Chat,
                                              text: "No tiene permiso para usar este comando."
                                              );
        }

        static String textorandom()
        {
            string texto = "Se quiso unir un bot, pero fue echado";
            Random randObj = new Random();

            switch (randObj.Next(6).ToString())
            {
                case "0":
                    texto += " con una patada en el traste.";
                    break;
                case "1":
                    texto += ". Ni thor levanta este BanMjolnir";
                    break;
                case "2":
                    texto += ". Geekout uso el BanHammer, es super efectivo!!";
                    break;
                case "3":
                    texto += ". Yo soy la ley #JudgeDredd";
                    break;
                case "4":
                    texto += ". No vuelve ni con las esferas del dragon";
                    break;
                default:
                    texto += ". He WONT be back #Schwarzenegger";
                    break;
            }
            

            return texto ;
        }
    }
}
