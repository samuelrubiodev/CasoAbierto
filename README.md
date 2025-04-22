# ⚙️ Installation

Follow these steps to get the **Active Case** project up and running:

1. **Download and run the backend server**  
   Clone and start the case‑management backend from [Active Case Server](https://github.com/samuelrubiodev/active-case-server).

2. **Install HashiCorp Vault**  
   Vault is used to manage your API keys securely. You have two options:  
   - **Local installation**: follow the instructions on the [official site](https://developer.hashicorp.com/vault/install).  
   - **Docker container** (recommended): pull and run the Vault image from [Docker Hub](https://hub.docker.com/r/hashicorp/vault).  
   > **Tip:** Using Docker isolates Vault from your host environment and keeps your system clean.

3. **Configure secrets in Vault**  
   1. **If using Docker**, open a shell in the running container:  
      ```bash
      docker exec -it vault sh
      ```  
   2. **Store your keys** (replace `YOUR_…_KEY` with the actual values):  
      ```bash
      vault kv put secret/keys \
        ELEVENLABS="YOUR_ELEVENLABS_KEY" \
        GROQ="YOUR_GROQ_KEY" \
        OPEN_ROUTER="YOUR_OPEN_ROUTER_KEY" \
        TOGETHER="YOUR_TOGETHER_KEY"
      ```

4. **Set environment variables**  
   Create a `.env` file or export them in your shell:  
   ```cmd
   set ACTIVE_CASE_HOST=localhost:3001
   ```
5. **Clone this repo**  
   ```bash
   git clone https://github.com/samuelrubiodev/CasoAbierto.git
   cd CasoAbierto
   ```
6. **🎮 ¡Open the game in Unity Editor!**
   
  
   
## LICENSE
The code is released under PolyForm‑Noncommercial‑1.0.0. Please read the license carefully in [LICENSE](./LICENSE.md).

## Third-party resources
All third‑party assets (videos, music, icons, SFX…) and their respective licenses are listed in [THIRD_PARTY.md](./THIRD_PARTY.md). Please review that file before using or contributing assets.
