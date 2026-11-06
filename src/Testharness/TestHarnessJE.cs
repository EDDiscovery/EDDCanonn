using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestHarness.Properties;

namespace CHash
{
    public partial class TestHarness 
    {

        private EDDDLLInterfaces.EDDDLLIF.JournalEntry MakeJE()
        {
            EDDDLLInterfaces.EDDDLLIF.JournalEntry nje = new EDDDLLInterfaces.EDDDLLIF.JournalEntry() { ver = 7, indexno = 19 };

            //v1
            nje.utctime = DateTime.UtcNow.ToString();
            nje.name = "EventSummary";
            nje.info = "Info";
            nje.detailedinfo = "DI";
            nje.materials = new string[2] { "one", "two" };
            nje.commodities = new string[2] { "c-one", "c-two" };
            nje.currentmissions = new string[2] { "m-one", "m-two" };
            nje.systemname = "Sys Fred";
            nje.x = 100.1;
            nje.y = 200.1;
            nje.z = 300.1;
            nje.travelleddistance = 1234.5;
            nje.travelledseconds = 6789;
            nje.islanded = true;
            nje.isdocked = true;
            nje.whereami = "Body";
            nje.shiptype = "Anaconda";
            nje.gamemode = "Open";
            nje.group = "Fred";
            nje.credits = 0xCCC12345678;
            nje.eventid = "FunEvent";
            nje.jid = 0xAAA12345678;
            nje.totalrecords = 2001;

            //v2
            nje.json = "{\"timestamp\"=\"10-20\"}";
            nje.cmdrname = "Buddy";
            nje.cmdrfid = "F19292";
            nje.shipident = "Y-1929";
            nje.shipname = "Julia";
            nje.hullvalue = 0x12345678;
            nje.rebuy = 0x1234000;
            nje.modulesvalue = 0x1234111;
            nje.stored = true;

            //v3
            nje.travelstate = "Travelling";
            nje.microresources = new string[] { "MR1", "MR2", "MR3" };

            // v4
            nje.horizons = false;
            nje.odyssey = true;
            nje.beta = false;

            //v5
            nje.wanted = false;
            nje.bodyapproached = true;
            nje.bookeddropship = false;
            nje.issrv = true;
            nje.isfighter = false;
            nje.onfoot = true;
            nje.bookedtaxi = false;

            nje.bodyname = "Bodyname";
            nje.bodytype = "Bodytype";
            nje.stationname = "stationname";
            nje.stationtype = "stationtype";
            nje.stationfaction = "stationfaction";
            nje.shiptypefd = "shiptypefd";
            nje.oncrewwithcaptain = "Captain Jack";
            nje.shipid = 0xa1a12345678;
            nje.bodyid = 2020;

            // v6

            nje.gameversion = "gamv1";
            nje.gamebuild = "10.2.3.4";

            // v7
            nje.fsdjumpnextsystemaddress = 1234;
            nje.fsdjumpnextsystemname = "Next sys";
            nje.systemaddress = 1234567;
            nje.marketid = 89012;
            nje.fullbodyid = 991234567;
            nje.loan = 1029292;
            nje.assets = 2902992929292;
            nje.currentboost = 2.3;
            nje.visits = 123;
            nje.multiplayer = true;
            nje.insupercruise = true;

            return nje;
        }


    }
}
