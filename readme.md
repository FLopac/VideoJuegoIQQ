# Video Juego Iquique · prototipo Playa Brava

Proyecto integrado para Unity **6000.4.2f1**. Incluye la escena, los sprites y los ajustes ya asignados.

1. Descomprime el ZIP en una carpeta nueva.
2. En Unity Hub, añade la carpeta que contiene Assets, Packages y ProjectSettings.
3. Abre el proyecto con 6000.4.2f1 y espera la importación.
4. Abre `Assets/Scenes/Playa_Brava_Prototipo.unity` y presiona Play. Usa **A/D** o las flechas para moverte y **Espacio** para saltar.

## Ajustes de esta versión

| Elemento | Configuración |
| --- | --- |
| Jugador | Dibujo de aproximadamente 1,2 unidades de alto. Collider de 0,42 × 1,2 y pies alineados con su base. Reposo, carrera, ascenso y descenso seleccionados por contacto con el suelo y velocidad. |
| Yeco | Aproximadamente 1,7 unidades de ancho al planear. Ocho cuadros de vuelo y giro del sprite al invertir el recorrido. |
| Heces | El hijo Visual quedó dentro del prefab que usa el lanzador. La gota mide unas 0,25 unidades de alto. |
| LanzadorYeco | Velocidad 6 unidades/s, disparo cada 1,8 s, dirección vertical hacia abajo. Para probar una dificultad más suave: 5 unidades/s y 2,2 s. |
| Cámara | Size 5,5, seguimiento horizontal suavizado, altura fija y límites calculados con el ancho de Suelo. |
| Extremos del mapa | Los colliders laterales llegan hasta el piso. El jugador conserva la caída vertical al tocar un muro y se aplica un material sin fricción al iniciar Play. |

Para ajustar tamaños, cambia el **Scale X/Y del hijo Visual** con valores iguales. Los dibujos de este prototipo usan 64 píxeles por unidad y las raíces de Jugador, Yeco y Heces están a escala 1. Si cambias mucho el tamaño visible, comprueba que el collider cubra el cuerpo sin incluir el espacio transparente del PNG.

Para balancear los disparos, ajusta **Projectile Speed** y **Seconds Between Shots** en `Yeco/LanzadorYeco`. La gota cambia de aspecto editando `Assets/Prefabs/Heces.prefab`.

## Prueba rápida en Unity

- Corre hacia ambos extremos: el jugador debe detenerse contra el muro y la cámara debe conservar el encuadre del mapa.
- Salta mientras mantienes la tecla hacia el muro: debe poder volver al suelo. Al invertir la dirección, debe poder alejarse.
- Corre, salta y aterriza sin soltar la tecla: comprueba los cambios de pose. Al soltarla, debe volver a reposo.
- Mira un ciclo completo del yeco: comprueba el giro y que las gotas salgan hacia abajo.
- Deja que una gota llegue al suelo y que otra golpee al jugador: deben desaparecer; el impacto en el jugador conserva el regreso al punto de inicio.

Se comprobaron las referencias de escena, prefab y sprites, las dimensiones de los colliders y el ZIP. La prueba en Play se hace en el editor; este paquete no se ejecutó en Unity aquí.

## Trabajar juntos con Git

1. Instalen **la versión exacta de `ProjectSettings/ProjectVersion.txt`**. Para esta copia es [Unity 6000.4.2f1](https://unity.com/releases/editor/whats-new/6000.4.2f1). Mantengan sus otras versiones instaladas si las usan en otros proyectos.
2. Esta copia ya incluye `.gitignore`, **Visible Meta Files** y **Force Text**. Mantengan los archivos `.meta` junto a sus assets y el contenido de Packages y ProjectSettings.
3. Crea un repositorio **privado y vacío** en GitHub. Si empiezas desde este ZIP y todavía no tienes un repositorio para el juego, abre una terminal en la raíz y ejecuta:

```bash
git init -b main
git add -- Assets Packages ProjectSettings .gitignore README.md
git commit -m "Base integrada de Playa Brava"
git remote add origin https://github.com/TU_USUARIO/VideoJuegoIquique.git
git push -u origin main
```

4. Invita a tu colega en **Settings → Collaborators → Add people**. Tras aceptar, puede clonar con `git clone https://github.com/TU_USUARIO/VideoJuegoIquique.git`, abrir la carpeta clonada en Hub y cargar la escena indicada arriba.
5. Antes de cada mejora, actualicen `main` con `git pull --ff-only` y creen una rama con `git switch -c mejora/nombre`. Acuerden quién editará la escena principal en cada ronda. El otro puede trabajar en sprites, scripts o prefabs; después integren los cambios mediante un pull request.

Si a tu colega le aparece una advertencia de versión, comparen primero el editor que Hub asignó al proyecto con ProjectVersion.txt. Si coincide y aún no abre, guarden el mensaje de Hub o el primer error rojo de Console para diagnosticarlo.

