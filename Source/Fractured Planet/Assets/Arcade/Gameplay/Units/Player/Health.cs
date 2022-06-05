﻿using UnityEngine;
using UnityEngine.UI;
// ReSharper disable All

namespace Complete
{
    public class Health : MonoBehaviour
    {
        public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
        public Slider m_Slider;                             // The slider to represent how much health the tank currently has.
        public Image m_FillImage;                           // The image component of the slider.
        public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
        public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
        public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.


        private AudioSource _explosionAudio;               // The audio source to play when the tank explodes.
        private ParticleSystem _explosionParticles;        // The particle system the will play when the tank is destroyed.

        public float _currentHealth;                      // How much health the tank currently has.

        public bool _dead;                                // Has the tank been reduced beyond zero health yet?


        private void Awake ()
        {
            // Instantiate the explosion prefab and get a reference to the particle system on it.
            _explosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem> ();

            // Get a reference to the audio source on the instantiated prefab.
            _explosionAudio = _explosionParticles.GetComponent<AudioSource> ();

            // Disable the prefab so it can be activated when it's required.
            _explosionParticles.gameObject.SetActive (false);
        }


        private void OnEnable()
        {
            // When the tank is enabled, reset the tank's health and whether or not it's dead.
            _currentHealth = m_StartingHealth;
            _dead = false;

            // Update the health slider's value and color.
            SetHealthUI();
        }


        public void TakeDamage (float amount)
        {
            // Reduce current health by the amount of damage done.
            _currentHealth -= amount;

            // Change the UI elements appropriately.
            SetHealthUI ();

            // If the current health is at or below zero and it has not yet been registered, call OnDeath.
            if (_currentHealth <= 0f && !_dead)
            {
                OnDeath ();
            }
        }


        private void SetHealthUI ()
        {
            // Set the slider's value appropriately.
            m_Slider.value = _currentHealth;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
            m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, _currentHealth / m_StartingHealth);
        }


        private void OnDeath ()
        {
            // Set the flag so that this function is only called once.
            _dead = true;

            // Move the instantiated explosion prefab to the tank's position and turn it on.
            _explosionParticles.transform.position = transform.position;
            _explosionParticles.gameObject.SetActive (true);

            // Play the particle system of the tank exploding.
            _explosionParticles.Play ();

            // Play the tank explosion sound effect.
            _explosionAudio.Play();

            // Turn the tank off.
            gameObject.SetActive (false);

            Destroy(this);
        }
    }
}
