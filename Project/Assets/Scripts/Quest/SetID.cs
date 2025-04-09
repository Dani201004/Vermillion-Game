using UnityEngine;

public class SetID : MonoBehaviour
{
    public int ID;
    //0 = Player, 1 = Ally, 2 = Enemy

    private void Start()
    {
        GetID();
    }

    void GetID()
    {
        if (this.gameObject.tag == "Player")
        { ID = 0; }
        else if (this.gameObject.tag == "Witch" || this.gameObject.tag == "Paladin" || this.gameObject.tag == "Cleric")
        { ID = 1; }
        else if (this.gameObject.tag == "Enemy")
        { ID = 2; }
    }
}
