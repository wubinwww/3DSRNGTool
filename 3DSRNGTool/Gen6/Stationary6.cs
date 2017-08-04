﻿using Pk3DSRNGTool.Core;

namespace Pk3DSRNGTool
{
    public class Stationary6 : StationaryRNG
    {
        private static uint getrand => RNGPool.getrand;
        private static uint rand(uint n) => (uint)(getrand * (ulong)n >> 32);
        private static void Advance(int n) => RNGPool.Advance(n);
        public bool InstantSync;
        public bool Bank;  // Bank = PokemonLink or Transporter
        public int Target; // Index of target pkm
        private bool tinysync => (InstantSync ? RNGPool.tinyframe?.rand2 : RNGPool.tinyframe?._sync) == true;
        private bool getSync => AlwaysSync || tinysync;

        public override RNGResult Generate()
        {
            Result6 rt = new Result6();
            rt.Level = Level;
            if (Bank)
                for (int i = Target; i > 1; i--)
                    Generate_Once();

            int StartFrame = RNGPool.index;

            //Sync
            rt.Synchronize = getSync;
            if (!AlwaysSync)
                Advance(60);

            //Encryption Constant
            rt.EC = getrand;

            //PID
            for (int i = PIDroll_count; i > 0; i--)
            {
                rt.PID = getrand;
                if (rt.PSV == TSV)
                {
                    if (IsShinyLocked)
                        rt.PID ^= 0x10000000;
                    else
                        rt.Shiny = true;
                    break;
                }
            }

            //IV
            rt.IVs = (int[])IVs.Clone();
            for (int i = PerfectIVCount; i > 0;)
            {
                uint tmp = rand(6);
                if (rt.IVs[tmp] < 0)
                {
                    i--; rt.IVs[tmp] = 31;
                }
            }
            for (int i = 0; i < 6; i++)
                if (rt.IVs[i] < 0)
                    rt.IVs[i] = (int)(getrand >> 27);

            //Ability
            rt.Ability = (byte)(Ability == 0 ? (getrand >> 31) + 1 : Ability);

            //Nature
            rt.Nature = (byte)(rt.Synchronize && Synchro_Stat < 25 ? Synchro_Stat : rand(25));

            //Gender
            rt.Gender = (byte)(RandomGender ? (rand(252) >= Gender ? 1 : 2) : Gender);

            //For Pokemon Link
            rt.FrameUsed = (byte)(RNGPool.index - StartFrame);

            return rt;
        }

        private void Generate_Once() // For link/Transporter
        {
            if (!IV3) // Johto starters
            {
                Advance(10); // EC + PID + IVs + Nature + Gender
                return;
            }
            Advance(2); // Link Legends/Transporter
            // Indefinite advance
            var IV = new bool[6];
            for (int i = 3; i > 0;)
            {
                uint tmp = rand(6);
                if (!IV[tmp])
                {
                    i--; IV[tmp] = true;
                }
            }
            Advance(Synchro_Stat < 25 ? 3 : 4); // Random IVs and Nature
            // No gender
        }

        public override void UseTemplate(Pokemon PM)
        {
            base.UseTemplate(PM);
            var pm6 = PM as PKM6;
            InstantSync = pm6.InstantSync;
            Bank = pm6.Bank;
            if (pm6.Bank && (pm6.Species == 151 || pm6.Species == 251 && pm6.Ability == 4))
                PerfectIVCount = 5;
        }
    }
}
