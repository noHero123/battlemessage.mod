using System;

using ScrollsModLoader.Interfaces;
using UnityEngine;
using Mono.Cecil;
//using Mono.Cecil;
//using ScrollsModLoader.Interfaces;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Reflection;
using JsonFx.Json;
using System.Text.RegularExpressions;
using System.Threading;


namespace BattleMessage.mod
{
    public class BattleMessageMod : BaseMod, ICommListener
	{

        private bool showEntryMessage = true;
        private bool startFullscreen = false;
        private bool makefullscreenback = false;

        public void handleMessage(Message msg)
        { // collect data for enchantments (or units who buff)

            //Console.WriteLine("###" + msg.GetType());

            if (msg is RoomChatMessageMessage)
            {
                
                
                RoomChatMessageMessage rcmm = (RoomChatMessageMessage)msg;
                if (this.showEntryMessage)
                {
                    if (rcmm.text.StartsWith("You have joined"))
                    {
                        

                        RoomChatMessageMessage nrcmm = new RoomChatMessageMessage(rcmm.roomName, "BattleMessage-Mod is active");
                        nrcmm.from = "BattleMessager";
                        App.ArenaChat.handleMessage(nrcmm);
                        this.showEntryMessage = false;
                        //App.Communicator.removeListener(this);

                    }
                }
            }


            if (msg is GameMatchMessage)
            {
                GameType gt = (msg as GameMatchMessage).gameType;
                if (gt == GameType.MP_LIMITED || gt == GameType.MP_QUICKMATCH || gt == GameType.MP_RANKED || gt == GameType.MP_UNRANKED)
                {
                    Console.WriteLine("GameMatchMessage");
                    this.startFullscreen = Screen.fullScreen;
                    Screen.SetResolution(Screen.width, Screen.height, true);
                    this.makefullscreenback = true;
                }
                //Dialogs.showNotification("BattleStarts", "an epic fight awaits you!");
            }


            if ((msg is OkMessage && (msg as OkMessage).op == "GameMatchDecline") || msg is BattleRedirectMessage)
            {
                Console.WriteLine("clicked decline or ok");
                if (this.makefullscreenback)
                {
                    Console.WriteLine("### make fullscreen back! " + this.startFullscreen);
                    Screen.SetResolution(Screen.width, Screen.height, this.startFullscreen);
                    this.makefullscreenback = false;
                }
                //Dialogs.showNotification("BattleStarts", "an epic fight awaits you!");
            }

            return;
        }


        public void onConnect(OnConnectData ocd)
        {
            return; // don't care
        }


		//initialize everything here, Game is loaded at this point
        public BattleMessageMod()
		{
            try
            {
                App.Communicator.addListener(this);
            }
            catch { }
		}

        

		public static string GetName ()
		{
			return "BattleMessage";
		}

		public static int GetVersion ()
		{
			return 1;
		}


       
		//only return MethodDefinitions you obtained through the scrollsTypes object
		//safety first! surround with try/catch and return an empty array in case it fails
		public static MethodDefinition[] GetHooks (TypeDefinitionCollection scrollsTypes, int version)
		{
            try
            {
                return new MethodDefinition[] {
                    scrollsTypes["Communicator"].Methods.GetMethod("send", new Type[]{typeof(Message)}),
             };
            }
            catch
            {
                return new MethodDefinition[] { };
            }
		}

      
       


        public override bool WantsToReplace(InvocationInfo info)
        {
            /*if (info.target is Card && info.targetMethod.Equals("getPieceKindText"))
            { return true; } */
            if (info.target is Communicator && info.targetMethod.Equals("send") && info.arguments[0] is RoomChatMessageMessage)
            {
                if (((RoomChatMessageMessage)info.arguments[0]).text.StartsWith("/bame "))
                {
                    return true;
                }
            }

            return false;
        }



        public override void ReplaceMethod(InvocationInfo info, out object returnValue)
        {
            returnValue = null;
            /*
            if (info.target is Card && info.targetMethod.Equals("getPieceKindText"))
            {
                string retu = (info.target as Card).getPieceKind().ToString();
                if (this.translatedPieceKind.ContainsKey(retu))
                { retu = translatedPieceKind[retu]; }
                returnValue = retu;
                if ((info.target as Card).isToken)
                {
                    returnValue = "TOKEN " + returnValue;
                }

            }*/
            
            if (info.target is Communicator && info.targetMethod.Equals("send") && info.arguments[0] is RoomChatMessageMessage && (info.arguments[0] as RoomChatMessageMessage).text.StartsWith("/bame "))
            {
                RoomChatMessageMessage rcmm = info.arguments[0] as RoomChatMessageMessage;
                rcmm.from = "BattleMessager";

                // CHANGE FONT
                if ((info.arguments[0] as RoomChatMessageMessage).text.StartsWith("/language font"))
                {

                    string choosenFont = rcmm.text.Replace("/language font ", "");
                    if (choosenFont == "arial")
                    {
                        rcmm.text ="Font was changed to arial";
                    }
                    App.ArenaChat.handleMessage(rcmm);
                    returnValue = true;
                    return;
                }

                
                App.ArenaChat.handleMessage(rcmm);
                returnValue = true;
                return;
            }
            
        }

       

        public override void BeforeInvoke(InvocationInfo info)
        {
            
            return;

        }

        public override void AfterInvoke (InvocationInfo info, ref object returnValue)
        {
            

            if (info.target is CardView && info.targetMethod.Equals("createHelpOverlay"))//createTexts
            {


            }
            //returnValue = null;
            return;//return false;
        }



        
	}
}
