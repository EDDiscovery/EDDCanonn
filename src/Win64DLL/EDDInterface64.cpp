#include "stdafx.h"  
#include "windows.h"  
#include "Objbase.h"
#include "EDDInterface64.h"
#include "stdio.h"
#include "strsafe.h"
#include <iostream>
#include <iomanip>
#include <fstream>

// STRINGs are in Unicode - this is a unicode DLL. 

using namespace std;

void WriteASCII(const char* str)		// ASCII file..
{
	fstream fstream("c:\\code\\eddif.txt", ios::app);         // open the file
	fstream << str;
	fstream.close();
}

void WriteUnicode(LPCTSTR buffer)
{
	char buffer2[30000];
	WideCharToMultiByte(CP_ACP, 0, buffer, -1, buffer2, sizeof(buffer2), 0, 0);		// need to convert back to ASCII
	WriteASCII((const char*)buffer2);
}

void WriteJournalEntry(JournalEntry ptr)
{
	if (ptr.indexno == -1)
	{
		WriteASCII("Journal entry is invalid\n");
		return;
	}

	int const arraysize = 100000;
	TCHAR buffer[arraysize];
	*buffer = 0;
	size_t bufsize = arraysize * sizeof(TCHAR);

	if (ptr.ver >= 1)
	{
		LPCTSTR pszFormat = TEXT("Version %d : UTC %s: %d:%s\nSummary:'%s'\nInfo:'%s'\nDetailed:'%s'\nSystem %s @ x%f y%f z%f\ntdist %f tsecs %u | landed %d docked %d | whereami '%s' ship '%s' gamemode %s group `%s` | %lld cr | jid %lld rec %d\n");
		TCHAR linebuf[1000];
		StringCbPrintfW(linebuf, sizeof(linebuf), pszFormat,
			ptr.ver,
			ptr.utctime, ptr.indexno, ptr.eventid,
			ptr.name, ptr.info, ptr.detailedinfo,
			ptr.systemname, ptr.x, ptr.y, ptr.z,
			ptr.travelleddistance, ptr.travelledseconds,
			ptr.islanded ? 1 : 0, ptr.isdocked ? 1 : 0,
			ptr.whereami, ptr.shiptype, ptr.gamemode, ptr.group, ptr.credits,
			ptr.jid, ptr.totalrecords);
		wcscat_s(buffer,arraysize, linebuf);

		wcscat_s(buffer, arraysize, TEXT("Materials:"));

		if (ptr.materials.cDims == 1)
		{

			if ((ptr.materials.fFeatures & FADF_BSTR) == FADF_BSTR)
			{
				BSTR* bstrArray;
				HRESULT hr = SafeArrayAccessData(&ptr.materials, (void**)&bstrArray);

				long iMin = 0;
				SafeArrayGetLBound(&ptr.materials, 1, &iMin);
				long iMax = 0;
				SafeArrayGetUBound(&ptr.materials, 1, &iMax);

				for (long i = iMin; i <= iMax; ++i)
				{
					wcscat_s(buffer, arraysize, bstrArray[i]);
					wcscat_s(buffer, arraysize, L",");		// WORDs! length
				}
			}
		}
		wcscat_s(buffer, arraysize, L"\n");

		wcscat_s(buffer, arraysize, L"Commodities:");

		if (ptr.commodities.cDims == 1)
		{
			if ((ptr.commodities.fFeatures & FADF_BSTR) == FADF_BSTR)
			{
				BSTR* bstrArray;
				HRESULT hr = SafeArrayAccessData(&ptr.commodities, (void**)&bstrArray);

				long iMin = 0;
				SafeArrayGetLBound(&ptr.commodities, 1, &iMin);
				long iMax = 0;
				SafeArrayGetUBound(&ptr.commodities, 1, &iMax);

				for (long i = iMin; i <= iMax; ++i)
				{
					wcscat_s(buffer, arraysize, bstrArray[i]);		// WORDs! length
					wcscat_s(buffer, arraysize, L",");		// WORDs! length
				}
			}
		}

		wcscat_s(buffer, arraysize, L"\n");

		wcscat_s(buffer, arraysize, L"Missions:");

		if (ptr.currentmissions.cDims == 1)
		{
			if ((ptr.currentmissions.fFeatures & FADF_BSTR) == FADF_BSTR)
			{
				BSTR* bstrArray;
				HRESULT hr = SafeArrayAccessData(&ptr.currentmissions, (void**)&bstrArray);

				long iMin = 0;
				SafeArrayGetLBound(&ptr.currentmissions, 1, &iMin);
				long iMax = 0;
				SafeArrayGetUBound(&ptr.currentmissions, 1, &iMax);

				for (long i = iMin; i <= iMax; ++i)
				{
					wcscat_s(buffer, arraysize, bstrArray[i]);		// WORDs! length
					wcscat_s(buffer, arraysize, L",");		// WORDs! length
				}
			}
		}

		wcscat_s(buffer, arraysize, L"\n");

	}

	if (ptr.ver >= 2)
	{
		LPCTSTR pszFormat = TEXT("V2: JSON:%s\nCmdr %s %s | ship %s %s | hv %lx rb %lx mv %lx | stored %d\n");
		TCHAR linebuf[10000];
		StringCbPrintfW(linebuf, sizeof(linebuf), pszFormat,
			ptr.json, ptr.cmdrname, ptr.cmdrfid,
			ptr.shipident, ptr.shipname,
			ptr.hullvalue, ptr.rebuy, ptr.modulesvalue, ptr.stored);
		wcscat_s(buffer, arraysize, linebuf);
	}

	if (ptr.ver >= 3)
	{
		LPCTSTR pszFormat = TEXT("V3: ts %s\n");
		TCHAR linebuf[1000];
		StringCbPrintfW(linebuf, sizeof(linebuf), pszFormat,
			ptr.travelstate);
		wcscat_s(buffer, arraysize, linebuf);

		wcscat_s(buffer, arraysize, L"MR:");

		if (ptr.microresources.cDims == 1)
		{
			if ((ptr.microresources.fFeatures & FADF_BSTR) == FADF_BSTR)
			{
				BSTR* bstrArray;
				HRESULT hr = SafeArrayAccessData(&ptr.microresources, (void**)&bstrArray);

				long iMin = 0;
				SafeArrayGetLBound(&ptr.microresources, 1, &iMin);
				long iMax = 0;
				SafeArrayGetUBound(&ptr.microresources, 1, &iMax);

				for (long i = iMin; i <= iMax; ++i)
				{
					wcscat_s(buffer, arraysize, bstrArray[i]);		// WORDs! length
					wcscat_s(buffer, arraysize, L",");		// WORDs! length
				}
			}
		}

		wcscat_s(buffer, arraysize, L"\n");
	}

	if (ptr.ver >= 4)
	{
		LPCTSTR pszFormat = TEXT("V4: h %d o%d b %d\n");
		TCHAR linebuf[1000];
		StringCbPrintfW(linebuf, sizeof(linebuf), pszFormat,
			ptr.horizons, ptr.odyssey, ptr.beta);
		wcscat_s(buffer, arraysize, linebuf);
	}


	if (ptr.ver >= 5)
	{
		LPCTSTR pszFormat = TEXT(
			"V5: w %d ba %d bdrop %d issrv %d isfig %d onfoot %d bookedtaxi %d\n"
			"bn %s bt %s sn %s st %s sf %s stype %s oncrew %s\n"
			"shipid %llx bodyid %d\n"
		);
		TCHAR linebuf[1000];
		StringCbPrintfW(linebuf, sizeof(linebuf), pszFormat,
			ptr.wanted, ptr.bodyapproached, ptr.bookeddropship, ptr.issrv, ptr.isfighter, ptr.onfoot, ptr.bookedtaxi,
			ptr.bodyname, ptr.bodytype, ptr.stationname, ptr.stationtype, ptr.stationfaction, ptr.shiptypefd, ptr.oncrewwithcaptain, ptr.shipid, ptr.bodyid);
		wcscat_s(buffer, arraysize, linebuf);
	}

	if (ptr.ver >= 6)
	{
		LPCTSTR pszFormat = TEXT(
			"V6: gv %s gb %s\n"
		);
		TCHAR linebuf[1000];
		StringCbPrintfW(linebuf, sizeof(linebuf), pszFormat,
			ptr.gameversion, ptr.gamebuild);
		wcscat_s(buffer, arraysize, linebuf);
	}

	if (ptr.ver >= 7)
	{
		LPCTSTR pszFormat = TEXT(
			"V7: fsdns %s fsdnextsysaddr %lld\n"
			"sa %lld marketid %lld fullbodyid %lld loan %lld assets %lld\n"
			"cb %f visits %d mp %d insuper %d\n"
		);
		TCHAR linebuf[1000];
		StringCbPrintfW(linebuf, sizeof(linebuf), pszFormat,
			ptr.fsdjumpnextsystemname,ptr.fsdjumpnextsystemaddress,
			ptr.systemaddress,ptr.marketid,ptr.fullbodyid,ptr.loan,ptr.assets,
			ptr.currentboost,ptr.visits,ptr.multiplayer,ptr.insupercruise
		);
		wcscat_s(buffer, arraysize, linebuf);
	}

	WriteASCII("--------------- Journal Entry Data\n");
	WriteUnicode(buffer);
	WriteASCII("--------------- END \n");
}

EDDCallBacks callbacks;

EDD_API BSTR EDDInitialise(BSTR ver, BSTR folder, EDDCallBacks pcallbacks)
{
	WriteASCII("\n\n============================\nInitialise:");
	WriteUnicode(ver);
	WriteASCII(",");
	WriteUnicode(folder);
	WriteASCII("\n");
	callbacks = pcallbacks;
	//MessageBoxW(0, ver, L"Caption ɑːkɒn", MB_OK);
	return SysAllocString(L"0.1.2.3;PLAYLASTFILELOAD");
	//return SysAllocString(L"!Error is this");		// for an error
}

EDD_API void EDDRefresh(BSTR Commander, JournalEntry ptr)
{
	int const arraysize = 3000;

	TCHAR buffer[arraysize];
	wcscpy_s(buffer, arraysize, L"\n**** Refresh:");
	wcscat_s(buffer, arraysize, Commander);
	wcscat_s(buffer, arraysize, L"\n");
	WriteUnicode(buffer);
	WriteJournalEntry(ptr);
}

EDD_API void EDDMainFormShown()
{
	WriteASCII("\n**** Main form shown\n");
}


EDD_API void EDDNewJournalEntry(JournalEntry ptr)
{
	WriteASCII("\n**** NewJournalEntry:\n");
	WriteJournalEntry(ptr);
}

EDD_API void EDDNewUnfilteredJournalEntry(JournalEntry ptr)
{
	WriteASCII("\n**** NewUnfilteredJournalEntry:\n");
	WriteJournalEntry(ptr);
}

EDD_API void EDDNewUIEvent(BSTR str)
{
	WriteASCII("\n**** NewUIEvent:\n");
	WriteUnicode(str);
}


EDD_API void EDDTerminate()
{
	WriteASCII("\n**** Terminate\n");
}

EDD_API BSTR EDDActionCommand(BSTR action, SAFEARRAY& args)		// should always return a string
{
	int const arraysize = 600;
	TCHAR buffer[arraysize];
	size_t cbDest = arraysize * sizeof(TCHAR);

	*buffer = 0;

	long iMin = 0;
	SafeArrayGetLBound(&args, 1, &iMin);
	long iMax = 0;
	SafeArrayGetUBound(&args, 1, &iMax);

	StringCbPrintfW(buffer, cbDest, L"\n**** Action Command %s: %d %d: ", action, iMin, iMax);

	BSTR* bstrArray;
	HRESULT hr = SafeArrayAccessData(&args, (void**)&bstrArray);

	for (long i = iMin; i <= iMax; ++i)
	{
		wcscat_s(buffer, arraysize, bstrArray[i]);
		wcscat_s(buffer, arraysize, L",");		// WORDs! length
	}

	WriteUnicode(buffer);
	WriteASCII("\n");

	if (wcscmp(action, L"HISTORY") == 0)
	{
		if (callbacks.RequestHistory != NULL)
		{
			for (int i = 0; i < 5; i++)
			{
				WriteASCII("call back:\n");
				SAFEARRAY sa;
				JournalEntry je = { 1,1,
									NULL,NULL,NULL,NULL,
									sa,sa, 
									NULL,0,0,0, 
									0,0, 
									0,0, 
									NULL,NULL,NULL,NULL, 0, 
									NULL, 
									sa,		// currentmissions
									0,0,
									NULL,NULL,NULL,NULL,NULL,0,0,0,0,
									NULL,
									sa,
				};
				(*callbacks.RequestHistory)(i, false, &je);		// perform a call back..
				WriteJournalEntry(je);
			}
		}
	}

	if (wcscmp(action, L"PROGRAM") == 0 && iMax == 1)
	{
		if (callbacks.RunAction != NULL)
		{
			WriteASCII("\nRun Action:\n");
			callbacks.RunAction(bstrArray[0], bstrArray[1]);
		}
	}

	return SysAllocString(L"+DLL Return value");
}

EDD_API void EDDActionJournalEntry(JournalEntry ptr)
{
	TCHAR buffer[1000];
	wcscpy_s(buffer, 1000, L"\n**** Journal Entry sent:\n");
	WriteUnicode(buffer);
	WriteJournalEntry(ptr);
}

EDD_API BSTR EDDConfig(BSTR istr, bool editit)
{
	TCHAR buffer[1000];
	if ( editit )
		wcscpy_s(buffer, 1000, L"\n**** Win64 Config Edit\n");
	else
		wcscpy_s(buffer, 1000, L"\n**** Win64 Config\n");
	WriteUnicode(buffer);
	*istr = (((*istr)+1) % 32) + '@';	// keep on changing it
	return SysAllocString(istr);
}

