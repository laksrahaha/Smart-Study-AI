const AIFollowUpQuestions = (() => {
  // USER STORY 3: Adaptive Follow-up Questions
  // This creates a follow-up short-answer question after a student gets a quiz question wrong.

  function shouldShowFollowUpButton(isCorrect) {
    return isCorrect === false;
  }

  async function generateFollowUpQuestion({
    topic,
    difficulty,
    question,
    selectedAnswer,
    correctAnswer,
    explanation,
    apiBase
  }) {
    const prompt = `
A student answered this quiz question incorrectly.

Topic: ${topic}
Difficulty: ${difficulty}

Original question:
${question}

Student's wrong answer:
${selectedAnswer}

Correct answer:
${correctAnswer}

Mistake explanation:
${explanation}

Create ONE short-answer follow-up question that helps the student practise the weak concept.
Only return the follow-up question.
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
        throw new Error("AI follow-up request failed.");
      }

      const data = await response.json();
      const followUpQuestion = data.result || data.Result || "";

      if (!followUpQuestion || followUpQuestion.includes("Gemini API Error")) {
        throw new Error("Gemini unavailable.");
      }

      return {
        question: followUpQuestion,
        usedFallback: false
      };

    } catch (error) {
      console.warn("Follow-up question fallback used:", error.message);

      return {
        question: createFallbackFollowUpQuestion({
          topic,
          difficulty,
          selectedAnswer,
          correctAnswer
        }),
        usedFallback: true
      };
    }
  }

  function createFallbackFollowUpQuestion({
    topic,
    difficulty,
    selectedAnswer,
    correctAnswer
  }) {
    return `In your own words, explain why "${correctAnswer}" is the better answer for this ${difficulty} ${topic} question, and why "${selectedAnswer}" was not correct.`;
  }

  function renderFollowUpQuestion({ container, followUpQuestion }) {
  container.innerHTML = `
    <div class="follow-up-box">
      <strong>Adaptive Follow-up Question</strong>
      <p class="follow-up-question-text">${escapeHtml(followUpQuestion)}</p>
      <label for="followUpAnswer">Your Answer:</label>
      <textarea id="followUpAnswer" rows="4" placeholder="Type your answer here..."></textarea>
    </div>
  `;
}

  function escapeHtml(str) {
    return String(str || "")
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");
  }

  return {
    shouldShowFollowUpButton,
    generateFollowUpQuestion,
    createFallbackFollowUpQuestion,
    renderFollowUpQuestion
  };
})();
