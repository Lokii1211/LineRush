using System.Collections.Generic;
using UnityEngine;

namespace _Game.UI
{
    /// <summary>
    /// Manages the energy orb display panel in the HUD.
    /// Replaces the original HeartPanel with a themed energy system.
    /// </summary>
    public class EnergyPanel : MonoBehaviour
    {
        [Header("Energy Orb References")]
        [SerializeField] private List<EnergyUI> _energyOrbs = new List<EnergyUI>();

        private bool _isInitialized = false;

        private int MaxEnergy
        {
            get
            {
                if (LivesManager.IsInitialized && LivesManager.Instance != null)
                {
                    return LivesManager.Instance.MaxLivesCount;
                }
                return 5; // Fallback default
            }
        }

        public void Initialize()
        {
            if (_isInitialized) return;

            int expectedOrbs = MaxEnergy;
            if (_energyOrbs.Count != expectedOrbs)
            {
                Debug.LogWarning($"{name}: Expected {expectedOrbs} energy orbs, but found {_energyOrbs.Count}. Please assign {expectedOrbs} EnergyUI components in Inspector.", this);
            }

            foreach (var orb in _energyOrbs)
            {
                if (orb != null)
                {
                    orb.Initialize();
                }
            }

            _isInitialized = true;
        }

        public void UpdateEnergy(int activeCharges)
        {
            for (int i = 0; i < _energyOrbs.Count; i++)
            {
                if (_energyOrbs[i] != null)
                {
                    bool isActive = i < activeCharges;
                    _energyOrbs[i].SetActive(isActive);
                }
            }
        }

        public void ResetEnergy()
        {
            UpdateEnergy(MaxEnergy);
        }
    }
}
