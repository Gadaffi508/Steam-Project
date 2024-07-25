# Steam-Project

Bu proje, Steam ile entegre edilmiş bir Unity oyunudur. Steam API'si kullanılarak oyuncuların oyun içinde etkileşime geçmesi sağlanır.

## Özellikler

- Steam ile entegrasyon
- Çok oyunculu oyun desteği
- Kullanıcı dostu arayüz

## Kurulum

1. Bu projeyi yerel makinenize klonlayın:
    ```bash
    git clone https://github.com/Gadaffi508/Steam-Project.git
    ```
2. Unity ile projeyi açın.
3. Gerekli paketleri yükleyin:
    - `Mirror`
    - `FizzySteamworks`
4. `steam_appid.txt` dosyasını düzenleyin ve kendi Steam App ID'nizi girin.

## Kullanım

### Oyun Başlatma

1. Unity Editor'ünde oyunu başlatın.
2. Steam üzerinden arkadaşlarınızla lobi oluşturun.
3. Çok oyunculu oyun deneyiminin keyfini çıkarın.

### Örnek Resimler

![1](https://github.com/user-attachments/assets/a689d998-3ae3-41c4-ac8c-54a2f7017206)

![2](https://github.com/user-attachments/assets/8b90d0b5-1be6-4aad-9875-1dfef9997d72)

### Örnek Kod

```csharp
using UnityEngine;
using Mirror;

public class PlayerController : NetworkBehaviour
{
    void Update()
    {
        if (!isLocalPlayer) return;

        // Hareket kodu
        float move = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.Translate(0, 0, move);
    }
}
