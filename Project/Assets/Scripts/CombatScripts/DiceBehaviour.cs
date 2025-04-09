using UnityEngine;

public class DiceBehaviour : MonoBehaviour
{
    private int numeroDado;
    private int resultadoDado;

    public void ConfigurarDado(int tipoDeDado)
    {
        numeroDado = tipoDeDado;
    }
    public int TirarDados()
    {
        resultadoDado = Random.Range(1, numeroDado + 1);

        return resultadoDado;
    }
    public int ResultadoDado
    {
        get { return resultadoDado; }
    }

}
