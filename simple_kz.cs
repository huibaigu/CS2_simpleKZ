using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace simplekz;

public class simplekzPlugin : BasePlugin
{
    public override string ModuleName => "simplekz";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Wangsir";
    public override string ModuleDescription => "Kz plugin that can only store read points";

    private UsersSettings?[] _usersSettings = new UsersSettings?[65];

    private bool enable;
    public override void Load(bool hotReload)
    {
        enable=true;
        RegisterListener<Listeners.OnClientConnected>(((slot) =>
        {
            _usersSettings[slot + 1] = new UsersSettings { index=-1,Location = new List<Vector>(), Rotation = new List<QAngle>()};
        }));
        RegisterListener<Listeners.OnClientDisconnectPost>(slot => _usersSettings[slot + 1] = null);
    }
    [ConsoleCommand("css_kz", "enable or disable")]
    public void OnSwitchCommand(CCSPlayerController? client, CommandInfo commandInfo)
    {
        if (client == null)enable=!enable;
        commandInfo.ReplyToCommand($"simplekz is {enable}");
    }

    [ConsoleCommand("css_v", "teleport location")]
    public void OnTeleportCommand(CCSPlayerController client, CommandInfo commandInfo)
    {
        if (client == null)return;
        if(enable&&_usersSettings[client.Index].index!=-1)
        {
            Vector local=_usersSettings[client.Index].Location[_usersSettings[client.Index].index];
            QAngle rotal=_usersSettings[client.Index].Rotation[_usersSettings[client.Index].index];
            client.PlayerPawn.Value!.Teleport(local,rotal,new Vector(0, 0, 0));
        }
    }
    [ConsoleCommand("css_pre", "teleport location")]
    public void OnPreCommand(CCSPlayerController client, CommandInfo commandInfo)
    {
        if (client == null)return;
        if(enable&&_usersSettings[client.Index].index!=-1)
        {
            _usersSettings[client.Index].index-=1;
            OnTeleportCommand(client,commandInfo);
        }
    }
    [ConsoleCommand("css_next", "teleport location")]
    public void OnNextCommand(CCSPlayerController client, CommandInfo commandInfo)
    {
        if (client == null)return;
        if(enable&&_usersSettings[client.Index].index<_usersSettings[client.Index].Location.Count-1)
        {
            _usersSettings[client.Index].index+=1;
            OnTeleportCommand(client,commandInfo);
        }
    }
    [ConsoleCommand("css_c", "storage location")]
    public void OnStorageCommand(CCSPlayerController client, CommandInfo commandInfo)
    {
        if (client == null)return;
        if(enable)
        {
            _usersSettings[client.Index].index++;
            if(_usersSettings[client.Index].index<=_usersSettings[client.Index].Location.Count-1)
            {
                _usersSettings[client.Index].Location.RemoveAt(_usersSettings[client.Index].Location.Count - 1);
                _usersSettings[client.Index].Rotation.RemoveAt(_usersSettings[client.Index].Rotation.Count - 1);
            }
            _usersSettings[client.Index].Location.Add(client.Pawn.Value.CBodyComponent.SceneNode.AbsOrigin*1);
            _usersSettings[client.Index].Rotation.Add(client.Pawn.Value.AbsRotation);
        }
    }
}
public class UsersSettings
{
    public List<Vector> Location { get; set; }
    public List<QAngle> Rotation { get; set; }
    public int index { get; set; }
}