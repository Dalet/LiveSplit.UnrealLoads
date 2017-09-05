#include <windows.h>

#define STATUS_NONE			0
#define STATUS_LOADING_MAP	1
#define STATUS_SAVING		2

#define DllExport	__declspec(dllexport)

typedef void *(__thiscall *t_LoadMap)(void*, const void*, void*, const void*, void*);
typedef void *(__thiscall *t_LoadMap_oldUnreal)(void*, const void*, void*, const void*, void*, void*);
typedef void *(__thiscall *t_LoadMap_SplinterCell)(void*, const void*, void*);
typedef void (__thiscall *t_LoadMap_SplinterCell3)(void*, const void*, void*);
typedef void (__thiscall *t_SaveGame)(void*, int);
typedef int (__thiscall *t_SaveGame_SplinterCell)(void*, const void*);
typedef void (__thiscall *t_SaveGame_SplinterCell3)(void*, void*, const void*);
typedef void(__thiscall *t_saveGame_DeusEx)(void*, void*, const void*);

t_LoadMap					g_oLoadMap;
t_LoadMap_oldUnreal			g_oLoadMap_oldUnreal;
t_LoadMap_SplinterCell		g_oLoadMap_SplinterCell;
t_LoadMap_SplinterCell3		g_oLoadMap_SplinterCell3;
t_SaveGame					g_oSaveGame;
t_SaveGame_SplinterCell		g_oSaveGame_SplinterCell;
t_SaveGame_SplinterCell3	g_oSaveGame_SplinterCell3;
t_saveGame_DeusEx	g_oSaveGame_DeusEx;

DllExport int		g_status = STATUS_NONE;
DllExport wchar_t	g_map[MAX_PATH];

DllExport
void set_map(const wchar_t *map)
{
	for (int i = 0; i < MAX_PATH; i++)
	{
		g_map[i] = map[i];
		if (map[i] == '\0')
			break;
	}
	g_map[MAX_PATH - 1] = '\0';
}

DllExport
void* __fastcall	Detour_LoadMap(void *This, void *edx, const void *URL, void *Pending, const void *TravelInfo, void *Error)
{
	wchar_t *map = *((wchar_t **)URL + 7);
	set_map(map);

	g_status = STATUS_LOADING_MAP;
	void *level = g_oLoadMap(This, URL, Pending, TravelInfo, Error);
	g_status = STATUS_NONE;

	return level;
}

DllExport
void* __fastcall	Detour_LoadMap_oldUnreal(void *This, void *edx, const void *URL, void *Pending, const void *TravelInfo, void *Error, void *UTravelDataManager)
{
	wchar_t *map = *((wchar_t **)URL + 7);
	set_map(map);

	g_status = STATUS_LOADING_MAP;
	void *level = g_oLoadMap_oldUnreal(This, URL, Pending, TravelInfo, Error, UTravelDataManager);
	g_status = STATUS_NONE;

	return level;
}

DllExport
void* __fastcall	Detour_LoadMap_SplinterCell(void *This, void *edx, const void *URL, void *Error)
{
	wchar_t *map = *((wchar_t **)URL + 7);
	set_map(map);

	g_status = STATUS_LOADING_MAP;
	void *level = g_oLoadMap_SplinterCell(This, URL, Error);
	g_status = STATUS_NONE;

	return level;
}

DllExport
void __fastcall	Detour_LoadMap_SplinterCell3(void *This, void *edx, const void *URL, void *Error)
{
	wchar_t *map = *((wchar_t **)URL + 7);
	set_map(map);

	g_status = STATUS_LOADING_MAP;
	g_oLoadMap_SplinterCell3(This, URL, Error);
	g_status = STATUS_NONE;
}

DllExport
void __fastcall		Detour_SaveGame(void *This, void *edx, int Position)
{
	g_status = STATUS_SAVING;
	g_oSaveGame(This, Position);
	g_status = STATUS_NONE;
}

DllExport
int __fastcall		Detour_SaveGame_SplinterCell(void *This, void *edx, const void *Position)
{
	g_status = STATUS_SAVING;
	int ret = g_oSaveGame_SplinterCell(This, Position);
	g_status = STATUS_NONE;
	return ret;
}

DllExport
void __fastcall		Detour_SaveGame_SplinterCell3(void *This, void *edx, void *ALevelInfo, const void *Position)
{
	g_status = STATUS_SAVING;
	g_oSaveGame_SplinterCell3(This, ALevelInfo, Position);
	g_status = STATUS_NONE;
}

DllExport
void __fastcall		Detour_SaveGame_DeusEx(void *This, void *a2, const void *a3)
{
	g_status = STATUS_SAVING;
	g_oSaveGame_DeusEx(This, a2, a3);
	g_status = STATUS_NONE;
}

int main()
{
	return 0;
}
