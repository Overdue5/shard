﻿using System;
using Server.Mobiles;

namespace Server.Custom.Games
{
    public class CustomGumpItem
    {
        private String m_Label;
        private String m_Message;

        private BaseGame m_Game;
        private PlayerMobile m_Player;

        public CustomGumpItem(String label, String message, BaseGame game)
        {
            Label = label;
            Message = message;
            Game = game;
        }

        public CustomGumpItem(String label, String message, BaseGame game, PlayerMobile player)
        {
            Game = game;
            Player = player;
            Label = label;
            Message = message;
        }

        public String Label
        {
            get => m_Label;
            set => m_Label = value;
        }

        public String Message
        {
            get => m_Message;
            set
            {
                m_Message = value;
                if (Game != null)
                {
                    if (Player == null)
                    {
                        Game.SendPlayerGumps();
                    }
                    else
                    {
                        Game.SendPlayerGump(Player);
                    }
                }
            }
        }

        public BaseGame Game
        {
            get => m_Game;
            set => m_Game = value;
        }

        public PlayerMobile Player
        {
            get => m_Player;
            set => m_Player = value;
        }

        public override string ToString()
        {
            return "CustomGumpItem: " + Label + " = " + Message;
        }
    }
}
