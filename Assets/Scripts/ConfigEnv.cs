using System;

public class ConfigEnv {
    public enum Envs
    {
        VAULT_HOST,
        VAULT_TOKEN,
        REDIS_HOST,
        REDIS_PASSWORD
    }

    public static string GetEnv(Envs env)
    {
        string envValue = Environment.GetEnvironmentVariable(env.ToString());
        if (string.IsNullOrEmpty(envValue))
        {
            throw new ArgumentNullException($"Environment variable {env} is not set.");
        }
        return envValue;
    }

}