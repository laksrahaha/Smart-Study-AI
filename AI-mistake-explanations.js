const AIMistakeExplanations = (() => {
  // USER STORY 2: Personalized Mistake Explanations
  // This file handles the explanation shown when a student chooses the wrong answer.
  // It uses the existing AI summarize endpoint, but also has fallback text if Gemini does not work locally.

  async function generateMistakeExplanation({
    question,
    selectedAnswer,
    correctAnswer,
    topic,
    difficulty,
    apiBase
  }) {
    const prompt = `
A student answered this multiple-choice question incorrectly.

Topic: ${topic}
Difficulty: ${difficulty}
${AIDifficultyAdjustment.getExplanationDifficultyInstruction(difficulty)}

Question:
${question}

Student's answer:
${selectedAnswer}

Correct answer:
${correctAnswer}

Write a short personalized explanation for the student.

Include:
1. Why the student's answer was not correct
2. Why the correct answer is better
3. One simple study tip

Keep the explanation clear, supportive, and easy to understand.
`;

    try {
      const response = await fetch(`${apiBase}/AI/quiz`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          content: prompt
        })
      });

      if (!response.ok) {
        throw new Error("AI explanation request failed.");
      }

      const data = await response.json();
      const explanation = data.result || data.Result || "";

      if (!explanation || explanation.includes("Gemini API Error")) {
        throw new Error("Gemini unavailable.");
      }

      return {
        explanation: explanation,
        usedFallback: false
      };

    } catch (error) {
      console.warn("Mistake explanation fallback used:", error.message);

      return {
        explanation: createFallbackExplanation({
          selectedAnswer,
          correctAnswer,
          topic,
          difficulty
        }),
        usedFallback: true
      };
    }
  }

  function createFallbackExplanation({
    selectedAnswer,
    correctAnswer,
    topic,
    difficulty
  }) {
    return `Your answer was "${selectedAnswer}", but the correct answer is "${correctAnswer}". This means you may need to review the ${topic} concept again. Since this was a ${difficulty} question, focus on the key idea first, then try another practice question to check your understanding.`;
  }

  return {
    generateMistakeExplanation,
    createFallbackExplanation
  };
})();
