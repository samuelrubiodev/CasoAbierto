public interface IPromptMaker<T> {
    T CreatePrompt(string input);
}