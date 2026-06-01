const AIDifficultyAdjustment = (() => {
  // USER STORY 4: AI Explanation and Question Difficulty Adjustment
  // This file controls how Easy, Medium and Hard difficulty should affect
  // both generated quiz questions and mistake explanations.

  function getQuestionDifficultyInstruction(difficulty) {
    if (difficulty === "Easy") {
      return `
Difficulty instruction:
Create beginner-friendly questions.
Use simple wording.
Focus on basic definitions and key terms from the notes.
Avoid tricky answer options.
`;
    }

    if (difficulty === "Hard") {
      return `
Difficulty instruction:
Create challenging questions.
Use deeper reasoning and application-based wording.
Make the incorrect options more realistic.
The question should require more than memorising one word.
`;
    }

    return `
Difficulty instruction:
Create medium-level questions.
Use clear wording but include some understanding and application.
The answer options should be reasonable but not too confusing.
`;
  }

  function getExplanationDifficultyInstruction(difficulty) {
    if (difficulty === "Easy") {
      return `
Explanation difficulty:
Explain the mistake in very simple words.
Use a supportive tone.
Keep the explanation short and beginner-friendly.
Give one simple study tip.
`;
    }

    if (difficulty === "Hard") {
      return `
Explanation difficulty:
Give a more detailed explanation.
Explain the concept behind the correct answer.
Mention why the wrong answer may seem correct but is not.
Give a stronger study tip for deeper revision.
`;
    }

    return `
Explanation difficulty:
Give a clear medium-detail explanation.
Explain why the answer was wrong and why the correct answer fits better.
Give one practical study tip.
`;
  }

  function getDifficultyLabel(difficulty) {
    if (difficulty === "Easy") {
      return "Easy - basic recall and simple understanding";
    }

    if (difficulty === "Hard") {
      return "Hard - deeper understanding and application";
    }

    return "Medium - balanced practice";
  }

  return {
    getQuestionDifficultyInstruction,
    getExplanationDifficultyInstruction,
    getDifficultyLabel
  };
})();