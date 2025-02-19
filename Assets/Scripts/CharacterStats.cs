﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    [Header("Mechanics")]
    public Camera MainCamera;
    public Animator animator;
    public Animator deathAnimator;
    public GameObject Muzzle;
    public GameObject LineRendererObject;
    public ParticleSystem particles; // Particle System use for firing
    public int MaxHP; // Player max health
    public int HP; // Player current health
    public int Damage; // Player damage per shot
    public float Accuracy; // The higher this is, the lower the accuracy
    public int Keys; // The number of keys the player currently holds
    public bool CanFire; // If the player has finished the current firing animation to fire again

    [Header("UI")]
    public Text KeyText;
    public Image HealthBar;

    void Start()
    {
        Cursor.visible = false;
        CanFire = true;
        updateHealthBar();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && CanFire)
        {
            CanFire = false;
            animator.SetTrigger("Fire");
        }
    }

    #region UI

    public void UpdateKeys(int num)
    {
        Keys += num;
        KeyText.text = "Keys: " + Keys;
    }

    private void updateHealthBar()
    {
        float fill = (float)HP / (float)MaxHP;
        HealthBar.fillAmount = fill;
    }

    #endregion

    private void fireWeapon()
    {
        int layerMask = 1 << 8;
        layerMask = ~layerMask;
        RaycastHit hit;
        Vector3 spreadVector = getSpreadVector();
        Debug.DrawRay(MainCamera.transform.position, spreadVector * 100, Color.red, 1800);
        if(Physics.Raycast(MainCamera.transform.position, spreadVector, out hit, 10000f, layerMask))
        {
            switch (hit.transform.tag)
            {
                case "AIBomber":
                    hit.transform.gameObject.GetComponent<AIBomber>().TakeDamage(Damage);
                    break;
                case "AIGunner":
                    hit.transform.gameObject.GetComponent<AIGunner>().TakeDamage(Damage);
                    break;
                case "AIShield":
                    hit.transform.gameObject.GetComponent<AIShield>().TakeDamage(Damage);
                    break;
                case "AIMortar":
                    hit.transform.gameObject.GetComponent<AIMortor>().TakeDamage(Damage);
                    break;
                case "Destructable":
                    hit.transform.gameObject.GetComponent<DestructableObject>().TakeDamage(Damage);
                    break;
                default:
                    Debug.Log(this.gameObject.name + ": CharacterStats -> Raycast hit returned unrecognised tag: " + hit.transform.tag + "      GameObject: " + hit.transform.gameObject.name);
                    break;
            }
            //var newLineRenderer = Instantiate(LineRendererObject, transform.position, Quaternion.identity);
            //newLineRenderer.GetComponent<ShootingLineRendererScript>().InitialiseLineRenderer(Muzzle.transform.position, hit.transform.position);
        }
    }

    private Vector3 getSpreadVector()
    {
        float yAngle = Random.Range(-Accuracy, Accuracy);
        float xAngle = Random.Range(-Accuracy, Accuracy);
        Quaternion ySpreadAngle = Quaternion.AngleAxis(yAngle, new Vector3(0, 1, 0));
        Quaternion xSpreadAngle = Quaternion.AngleAxis(xAngle, new Vector3(1, 0, 0));
        Quaternion totalSpreadAngle = xSpreadAngle * ySpreadAngle;
        Vector3 newVector = totalSpreadAngle * MainCamera.transform.forward;
        return newVector;
    }

    public void SetCanFire()
    {
        CanFire = true;
    }

    public bool RecoverHealth(int recovery)
    {
        if (HP >= MaxHP)
            return false;
        HP += recovery;
        if (HP > MaxHP)
            HP = MaxHP;
        updateHealthBar();
        return true;
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        updateHealthBar();
        if (HP <= 0)
            playerDeath();
    }

    public void playerDeath()
    {
        Debug.Log("Player Dead!");
        deathAnimator.SetTrigger("Die");
    }
}
