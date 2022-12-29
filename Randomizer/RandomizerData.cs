﻿using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;
using Newtonsoft.Json.Linq;

namespace Randomizer;

public class RandomizerData
{
    private readonly Dictionary<string, object> _settings;

    public RandomizerData(IDictionary<string, object> settings, string seed, int slot = 1)
    {
        _settings = new Dictionary<string, object>(settings);
        Seed = seed;
        Slot = slot;

        // Initialize our locations dictionaries.
        foreach (var jToken in (JArray) _settings["active_locations"])
        {
            var obj = (JObject) jToken;
            var item = new NetworkItem
            {
                Item = Convert.ToInt64(obj["item"]),
                Location = Convert.ToInt64(obj["location"]),
                Player = Convert.ToInt32(obj["player"]),
                Flags = (ItemFlags) Convert.ToInt32(obj["flags"])
            };
            ActiveLocations.Add(item.Location, item);
            CheckedLocations.Add(item.Location, false);
        }

        // Add our starting inventory into memory so we can collect them on a new save.
        foreach (var item in (JArray) _settings["starting_inventory"])
            StartingInventory.Add(Convert.ToInt64(item));
    }

    public Dictionary<long, NetworkItem> ActiveLocations   { get; } = new();
    public Dictionary<long, bool>        CheckedLocations  { get; } = new();
    public List<long>                    StartingInventory { get; } = new();
    public string                        Seed              { get; }
    public int                           Slot              { get; }

    public bool StartingGender        => Convert.ToBoolean(_settings["starting_gender"]);
    public byte StartingClass         => Convert.ToByte(_settings["starting_class"]);
    public int  NewGamePlus           => Convert.ToInt32(_settings["new_game_plus"]);
    public int  ChestsPerZone         => Convert.ToInt32(_settings["chests_per_zone"]);
    public int  FairyChestsPerZone    => Convert.ToInt32(_settings["fairy_chests_per_zone"]);
    public bool UniversalChests       => Convert.ToBoolean(_settings["universal_chests"]);
    public bool UniversalFairyChests  => Convert.ToBoolean(_settings["universal_fairy_chests"]);
    public int  ArchitectFee          => Convert.ToInt32(_settings["architect_fee"]);
    public bool DisableCharon         => Convert.ToBoolean(_settings["disable_charon"]);
    public bool RequirePurchasing     => Convert.ToBoolean(_settings["require_purchasing"]);
    public int  NumberOfChildren      => Convert.ToInt32(_settings["number_of_children"]);
    public bool FreeDiaryOnGeneration => Convert.ToBoolean(_settings["free_diary_on_generation"]);
    public bool ChallengeKhidr        => Convert.ToBoolean(_settings["khidr"]);
    public bool ChallengeAlexander    => Convert.ToBoolean(_settings["alexander"]);
    public bool ChallengeLeon         => Convert.ToBoolean(_settings["leon"]);
    public bool ChallengeHerodotus    => Convert.ToBoolean(_settings["herodotus"]);
    public bool DeathLink             => Convert.ToBoolean(_settings["death_link"]);

    public float GoldGainMultiplier => Convert.ToInt32(_settings["gold_gain_multiplier"]) switch
    {
        0 => 1f, // Normal
        1 => 0.25f, // Quarter
        2 => 0.5f, // Half
        3 => 2f, // Double
        4 => 4f, // Quadruple
        _ => 1f
    };
}
