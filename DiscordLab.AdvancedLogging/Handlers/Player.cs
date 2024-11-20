using System.Globalization;
using System.Reflection;
using DiscordLab.AdvancedLogging.API.Features;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PluginAPI.Events;
using Event = DiscordLab.AdvancedLogging.API.Attributes.Event;

namespace DiscordLab.AdvancedLogging.Handlers;

public class Player : IRegisterable
{
    private List<(EventInfo EventInfo, Delegate Handler)> _events;
    
    private static Translation translation => Plugin.Instance.Translation;
    private static Config config => Plugin.Instance.Config;
    
    public void Init()
    {
        _events = new();
        IEnumerable<MethodInfo> events = GetType()
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.GetCustomAttributes(typeof(Event), false).Length > 0);
        foreach (MethodInfo mEvent in events)
        {
            Event eventAttribute = (Event)mEvent.GetCustomAttributes(typeof(Event), false).First();
            string eventName = eventAttribute.Name;
            ulong id = DiscordBot.Instance.GetChannelId("Player", eventName);
            if(id == 0) continue;
            EventInfo eventInfo = typeof(Exiled.Events.Handlers.Player).GetEvent(eventName);
            if (eventInfo == null) continue;
            Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, mEvent);
            eventInfo.AddEventHandler(null, handler);
            _events.Add((eventInfo, handler));
        }
    }
    
    public void Unregister()
    {
        foreach (var (eventInfo, handler) in _events)
        {
            eventInfo.RemoveEventHandler(null, handler);
        }
        _events = null;
    }
    
    [Event("ReservedSlot")]
    private void OnReservedSlot(ReservedSlotsCheckEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ReservedSlot, "Player", "ReservedSlot")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ReservedSlot, ev));
    }
    
    [Event("Kicked")]
    private void OnKicked(KickedEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Kicked, "Player", "Kicked")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Kicked, ev));
    }
    
    [Event("Banned")]
    private void OnBanned(BannedEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Banned, "Player", "Banned")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Banned, ev));
    }
    
    [Event("UsedItem")]
    private void OnUsedItem(UsedItemEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.UsedItem, "Player", "UsedItem")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.UsedItem, ev));
    }
    
    [Event("Interacted")]
    private void OnInteracted(InteractedEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Interacted, "Player", "Interacted")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Interacted, ev));
    }
    
    [Event("ActivatingWarheadPanel")]
    private void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ActivatingWarheadPanel, "Player", "ActivatingWarheadPanel")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ActivatingWarheadPanel, ev));
    }
    
    [Event("ActivatingWorkstation")]
    private void OnActivatingWorkstation(ActivatingWorkstationEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ActivatingWorkstation, "Player", "ActivatingWorkstation")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ActivatingWorkstation, ev));
    }
    
    [Event("Verified")]
    private void OnVerified(VerifiedEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Verified, "Player", "Verified")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Verified, ev));
    }
    
    [Event("Left")]
    private void OnLeft(LeftEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Left, "Player", "Left")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Left, ev));
    }
    
    [Event("Hurt")]
    private void OnHurt(HurtEventArgs ev)
    {
        HurtQueue(ev);
    }
    
    private List<HurtQueueItem> _hurtQueue;
    
    private void HurtQueue(HurtEventArgs ev)
    {
        _hurtQueue ??= new();
        HurtQueueItem hurt = 
            _hurtQueue.FirstOrDefault(h => h.Player == ev.Player);
        if (hurt == null)
        {
            HurtQueueItemAttacker attacker = new();
            if (ev.Attacker != null)
            {
                attacker.Attacker = ev.Attacker;
                attacker.Damage = ev.Amount;
            }

            HurtQueueItem item = new()
            {
                Player = ev.Player,
                Attackers = new() { attacker }
            };
            
            _hurtQueue.Add(item);
        }
        else if (ev.Attacker != null)
        {
            HurtQueueItemAttacker attacker = new()
            {
                Attacker = ev.Attacker,
                Damage = ev.Amount
            };
            hurt.Attackers.Add(attacker);
        }

        if (_hurtQueue.Count == 1)
        {
            Timing.CallDelayed(15, () =>
            {
                List<string> toSend = _hurtQueue
                    .SelectMany(item => item.Attackers, (item, attacker) => new
                    {
                        Player = item.Player.Nickname,
                        Attacker = attacker.Attacker.Nickname,
                        Damage = attacker.Damage.ToString(CultureInfo.InvariantCulture)
                    })
                    .Select(replacement => translation.Player.Hurt
                        .Replace("{Player}", replacement.Player)
                        .Replace("{Attacker}", replacement.Attacker)
                        .Replace("{Damage}", replacement.Damage))
                    .ToList();

                _hurtQueue.Clear();
                DiscordBot.Instance.GetChannelById(config.Player.Hurt, "Player", "Hurt")
                    .SendMessageAsync(string.Join("\n", toSend));
            });
        }
    }
    
    [Event("Healed")]
    private void OnHealed(HealedEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Healed, "Player", "Healed")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Healed, ev));
    }
    
    [Event("Died")]
    private void OnDied(DiedEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Died, "Player", "Died")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Died, ev));
    }
    
    [Event("ChangingRole")]
    private void OnChangingRole(ChangingRoleEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ChangingRole, "Player", "ChangingRole")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ChangingRole, ev));
    }
    
    [Event("ThrownProjectile")]
    private void OnThrownProjectile(ThrownProjectileEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ThrownProjectile, "Player", "ThrownProjectile")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ThrownProjectile, ev));
    }
    
    [Event("DroppedItem")]
    private void OnDroppedItem(DroppedItemEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.DroppedItem, "Player", "DroppedItem")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.DroppedItem, ev));
    }
    
    [Event("PickingUpItem")]
    private void OnPickingUpItem(PickingUpItemEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.PickingUpItem, "Player", "PickingUpItem")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.PickingUpItem, ev));
    }
    
    [Event("Handcuffing")]
    private void OnHandcuffing(HandcuffingEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Handcuffing, "Player", "Handcuffing")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Handcuffing, ev));
    }
    
    [Event("RemovingHandcuffs")]
    private void OnRemovingHandcuffs(RemovingHandcuffsEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.RemovedHandcuffs, "Player", "RemovedHandcuffs")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.RemovedHandcuffs, ev));
    }
    
    [Event("Escaping")]
    private void OnEscaping(EscapingEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Escaping, "Player", "Escaping")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Escaping, ev));
    }
    
    [Event("Shot")]
    private void OnShot(ShotEventArgs ev)
    {
        _shotQueue ??= new();
        if(ev.Target == null) return;
        _shotQueue.Add(ev);
        if (_shotQueue.Count == 1)
        {
            Timing.CallDelayed(15, () =>
            {
                List<string> toSend = _shotQueue
                    .Select(replacement => translation.Player.Shot
                        .Replace("{Player}", replacement.Player.Nickname)
                        .Replace("{Target}", replacement.Target.Nickname)
                        .Replace("{Damage}", replacement.Damage.ToString(CultureInfo.InvariantCulture)))
                    .ToList();

                _shotQueue.Clear();
                DiscordBot.Instance.GetChannelById(config.Player.Shot, "Player", "Shot")
                    .SendMessageAsync(string.Join("\n", toSend));
            });
        }
    }
    
    private List<ShotEventArgs> _shotQueue;
    
    [Event("EnteringPocketDimension")]
    private void OnEnteringPocketDimension(EnteringPocketDimensionEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.EnteringPocketDimension, "Player", "EnteringPocketDimension")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.EnteringPocketDimension, ev));
    }
    
    [Event("EscapingPocketDimension")]
    private void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.EscapingPocketDimension, "Player", "EscapingPocketDimension")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.EscapingPocketDimension, ev));
    }
    
    [Event("FailingEscapePocketDimension")]
    private void OnFailingEscapePocketDimension(FailingEscapePocketDimensionEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.FailingEscapePocketDimension, "Player", "FailingEscapePocketDimension")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.FailingEscapePocketDimension, ev));
    }
    
    [Event("ReloadingWeapon")]
    private void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ReloadingWeapon, "Player", "ReloadingWeapon")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ReloadingWeapon, ev));
    }
    
    [Event("Spawned")]
    private void OnSpawned(SpawnedEventArgs ev)
    {
        if (ev.Reason is SpawnReason.RoundStart or SpawnReason.Respawn) return;
        DiscordBot.Instance.GetChannelById(config.Player.Spawned, "Player", "Spawned")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Spawned, ev));
    }
    
    [Event("ChangedItem")]
    private void OnChangedItem(ChangedItemEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ChangedItem, "Player", "ChangedItem")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ChangedItem, ev));
    }
    
    [Event("ChangingGroup")]
    private void OnChangingGroup(ChangingGroupEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ChangingGroup, "Player", "ChangingGroup")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ChangingGroup, ev));
    }
    
    [Event("InteractingDoor")]
    private void OnInteractingDoor(InteractingDoorEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.InteractingDoor, "Player", "InteractingDoor")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.InteractingDoor, ev));
    }
    
    [Event("InteractingElevator")]
    private void OnInteractingElevator(InteractingElevatorEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.InteractingElevator, "Player", "InteractingElevator")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.InteractingElevator, ev));
    }
    
    [Event("InteractingLocker")]
    private void OnInteractingLocker(InteractingLockerEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.InteractingLocker, "Player", "InteractingLocker")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.InteractingLocker, ev));
    }
    
    [Event("TriggeringTesla")]
    private void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.TriggeringTesla, "Player", "TriggeringTesla")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.TriggeringTesla, ev));
    }
    
    [Event("UnlockingGenerator")]
    private void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.UnlockingGenerator, "Player", "UnlockingGenerator")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.UnlockingGenerator, ev));
    }
    
    [Event("OpeningGenerator")]
    private void OnOpeningGenerator(OpeningGeneratorEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.OpeningGenerator, "Player", "OpeningGenerator")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.OpeningGenerator, ev));
    }
    
    [Event("ClosingGenerator")]
    private void OnClosingGenerator(ClosingGeneratorEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ClosingGenerator, "Player", "ClosingGenerator")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ClosingGenerator, ev));
    }
    
    [Event("ActivatingGenerator")]
    private void OnActivatingGenerator(ActivatingGeneratorEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ActivatingGenerator, "Player", "ActivatingGenerator")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ActivatingGenerator, ev));
    }
    
    [Event("StoppingGenerator")]
    private void OnStoppingGenerator(StoppingGeneratorEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.StoppingGenerator, "Player", "StoppingGenerator")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.StoppingGenerator, ev));
    }
    
    [Event("ReceivingEffect")]
    private void OnReceivingEffect(ReceivingEffectEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ReceivingEffect, "Player", "ReceivingEffect")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ReceivingEffect, ev));
    }
    
    [Event("IssuingMute")]
    private void OnIssuingMute(IssuingMuteEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.IssuingMute, "Player", "IssuingMute")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.IssuingMute, ev));
    }
    
    [Event("RevokingMute")]
    private void OnRevokingMute(RevokingMuteEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.RevokingMute, "Player", "RevokingMute")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.RevokingMute, ev));
    }
    
    [Event("UsingRadioBattery")]
    private void OnUsingRadioBattery(UsingRadioBatteryEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.UsingRadioBattery, "Player", "UsingRadioBattery")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.UsingRadioBattery, ev));
    }
    
    [Event("ChangingRadioPreset")]
    private void OnChangingRadioPreset(ChangingRadioPresetEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ChangingRadioPreset, "Player", "ChangingRadioPreset")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ChangingRadioPreset, ev));
    }
    
    [Event("ChangingMicroHIDState")]
    private void OnChangingMicroHIDState(ChangingMicroHIDStateEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ChangingMicroHIDState, "Player", "ChangingMicroHIDState")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ChangingMicroHIDState, ev));
    }
    
    [Event("UsingMicroHIDEnergy")]
    private void OnUsingMicroHIDEnergy(UsingMicroHIDEnergyEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.UsingMicroHIDEnergy, "Player", "UsingMicroHIDEnergy")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.UsingMicroHIDEnergy, ev));
    }
    
    [Event("DroppedAmmo")]
    private void OnDroppedAmmo(DroppedAmmoEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.DroppedAmmo, "Player", "DroppedAmmo")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.DroppedAmmo, ev));
    }
    
    [Event("InteractingShootingTarget")]
    private void OnInteractingShootingTarget(InteractingShootingTargetEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.InteractingShootingTarget, "Player", "InteractingShootingTarget")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.InteractingShootingTarget, ev));
    }
    
    [Event("DamagingShootingTarget")]
    private void OnDamagingShootingTarget(DamagingShootingTargetEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.DamagingShootingTarget, "Player", "DamagingShootingTarget")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.DamagingShootingTarget, ev));
    }
    
    [Event("FlippingCoin")]
    private void OnFlippingCoin(FlippingCoinEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.FlippingCoin, "Player", "FlippingCoin")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.FlippingCoin, ev));
    }
    
    [Event("TogglingFlashlight")]
    private void OnTogglingFlashlight(TogglingFlashlightEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.TogglingFlashlight, "Player", "TogglingFlashlight")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.TogglingFlashlight, ev));
    }
    
    [Event("UnloadingWeapon")]
    private void OnUnloadingWeapon(UnloadingWeaponEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.UnloadingWeapon, "Player", "UnloadingWeapon")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.UnloadingWeapon, ev));
    }
    
    [Event("AimingDownSight")]
    private void OnAimingDownSight(AimingDownSightEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.AimingDownSight, "Player", "AimingDownSight")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.AimingDownSight, ev));
    }
    
    [Event("TogglingWeaponFlashlight")]
    private void OnTogglingWeaponFlashlight(TogglingWeaponFlashlightEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.TogglingWeaponFlashlight, "Player", "TogglingWeaponFlashlight")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.TogglingWeaponFlashlight, ev));
    }
    
    [Event("DryfiringWeapon")]
    private void OnDryfiringWeapon(DryfiringWeaponEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.DryfiringWeapon, "Player", "DryfiringWeapon")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.DryfiringWeapon, ev));
    }
    
    [Event("VoiceChatting")]
    private void OnVoiceChatting(VoiceChattingEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.VoiceChatting, "Player", "VoiceChatting")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.VoiceChatting, ev));
    }
    
    [Event("MakingNoise")]
    private void OnMakingNoise(MakingNoiseEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.MakingNoise, "Player", "MakingNoise")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.MakingNoise, ev));
    }
    
    [Event("Jumping")]
    private void OnJumping(JumpingEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Jumping, "Player", "Jumping")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Jumping, ev));
    }
    
    [Event("Landing")]
    private void OnLanding(LandingEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Landing, "Player", "Landing")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Landing, ev));
    }
    
    [Event("Transmitting")]
    private void OnTransmitting(TransmittingEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.Transmitting, "Player", "Transmitting")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.Transmitting, ev));
    }
    
    [Event("ChangingMoveState")]
    private void OnChangingMoveState(ChangingMoveStateEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ChangingMoveState, "Player", "ChangingMoveState")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ChangingMoveState, ev));
    }
    
    [Event("ChangingSpectatedPlayer")]
    private void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ChangingSpectatedPlayer, "Player", "ChangingSpectatedPlayer")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ChangingSpectatedPlayer, ev));
    }
    
    [Event("TogglingNoClip")]
    private void OnTogglingNoClip(TogglingNoClipEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.TogglingNoClip, "Player", "TogglingNoClip")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.TogglingNoClip, ev));
    }
    
    [Event("TogglingOverwatch")]
    private void OnTogglingOverwatch(TogglingOverwatchEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.TogglingOverwatch, "Player", "TogglingOverwatch")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.TogglingOverwatch, ev));
    }
    
    [Event("TogglingRadio")]
    private void OnTogglingRadio(TogglingRadioEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.TogglingRadio, "Player", "TogglingRadio")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.TogglingRadio, ev));
    }
    
    [Event("SearchingPickup")]
    private void OnSearchingPickup(SearchingPickupEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.SearchingPickup, "Player", "SearchingPickup")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.SearchingPickup, ev));
    }
    
    [Event("SendingAdminChatMessage")]
    private void OnSendingAdminChatMessage(SendingAdminChatMessageEventsArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.SendingAdminChatMessage, "Player", "SendingAdminChatMessage")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.SendingAdminChatMessage, ev));
    }
    
    [Event("PlayerDamageWindow")]
    private void OnPlayerDamageWindow(PlayerDamagedWindowEvent ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.PlayerDamageWindow, "Player", "PlayerDamageWindow")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.PlayerDamageWindow, ev));
    }
    
    [Event("DamagingDoor")]
    private void OnDamagingDoor(DamagingDoorEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.DamagingDoor, "Player", "DamagingDoor")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.DamagingDoor, ev));
    }
    
    [Event("ItemAdded")]
    private void OnItemAdded(ItemAddedEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ItemAdded, "Player", "ItemAdded")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ItemAdded, ev));
    }
    
    [Event("ItemRemoved")]
    private void OnItemRemoved(ItemRemovedEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ItemRemoved, "Player", "ItemRemoved")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ItemRemoved, ev));
    }
    
    [Event("EnteringEnvironmentalHazard")]
    private void OnEnteringEnvironmentalHazard(EnteringEnvironmentalHazardEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.EnteringEnvironmentalHazard, "Player", "EnteringEnvironmentalHazard")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.EnteringEnvironmentalHazard, ev));
    }
    
    [Event("StayingOnEnvironmentalHazard")]
    private void OnStayingOnEnvironmentalHazard(StayingOnEnvironmentalHazardEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.StayingOnEnvironmentalHazard, "Player", "StayingOnEnvironmentalHazard")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.StayingOnEnvironmentalHazard, ev));
    }
    
    [Event("ExitingEnvironmentalHazard")]
    private void OnExitingEnvironmentalHazard(ExitingEnvironmentalHazardEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ExitingEnvironmentalHazard, "Player", "ExitingEnvironmentalHazard")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ExitingEnvironmentalHazard, ev));
    }
    
    [Event("ChangingNickname")]
    private void OnChangingNickname(ChangingNicknameEventArgs ev)
    {
        DiscordBot.Instance.GetChannelById(config.Player.ChangingNickname, "Player", "ChangingNickname")
            .SendMessageAsync(StringReplacer.Replacer(translation.Player.ChangingNickname, ev));
    }
}