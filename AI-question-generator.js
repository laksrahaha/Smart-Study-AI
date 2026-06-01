const AIQuestionGenerator = (() => {
  async function generateQuestions({ topic, difficulty, notes, apiBase }) {
    const noteContent = notes
      .map(note => `${note.title}: ${note.content}`)
      .join("\n\n");

    const prompt = `
Create 3 multiple-choice quiz questions for a student.

Topic: ${topic}
Difficulty: ${difficulty}
${AIDifficultyAdjustment.getQuestionDifficultyInstruction(difficulty)}

Use these study notes:
${noteContent}

Return ONLY valid JSON in this exact format:
[
  {
    "question": "question text here",
    "options": ["option A", "option B", "option C", "option D"],
    "correctAnswer": "exact correct option text"
  }
]

Rules:
- Each question must have exactly 4 options.
- The correctAnswer must exactly match one of the options.
- Do not include markdown.
- Do not include explanation.
`;

    try {
      const response = await fetch(`${apiBase}/AI/summarize`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          content: prompt
        })
      });

      if (!response.ok) {
        throw new Error("AI request failed.");
      }

      const data = await response.json();
      const aiText = data.result || data.Result || "";

      if (!aiText || aiText.includes("Gemini API Error")) {
        throw new Error("Gemini unavailable.");
      }

      const parsedQuestions = parseAIQuestions(aiText);

      if (parsedQuestions.length === 0) {
        throw new Error("AI response could not be converted into quiz questions.");
      }

      return {
        questions: parsedQuestions,
        usedFallback: false
      };

    } catch (error) {
      console.warn("AI question generator fallback used:", error.message);

      return {
        questions: createFallbackQuestions(topic, difficulty, notes),
        usedFallback: true
      };
    }
  }

  function parseAIQuestions(aiText) {
    try {
      let cleaned = String(aiText).trim();

      cleaned = cleaned.replace(/^```json/i, "");
      cleaned = cleaned.replace(/^```/i, "");
      cleaned = cleaned.replace(/```$/i, "");
      cleaned = cleaned.trim();

      const firstBracket = cleaned.indexOf("[");
      const lastBracket = cleaned.lastIndexOf("]");

      if (firstBracket !== -1 && lastBracket !== -1) {
        cleaned = cleaned.substring(firstBracket, lastBracket + 1);
      }

      const parsed = JSON.parse(cleaned);

      return parsed
        .filter(question =>
          question.question &&
          Array.isArray(question.options) &&
          question.options.length === 4 &&
          question.correctAnswer
        )
        .map(question => ({
          question: question.question,
          options: question.options,
          correctAnswer: question.correctAnswer
        }));

    } catch (error) {
      console.error("Could not parse AI quiz response:", error);
      return [];
    }
  }

  function createFallbackQuestions(topic, difficulty, notes) {
    const questions = [];

    notes.slice(0, 3).forEach(note => {
      const words = (note.content || "")
        .split(/\s+/)
        .map(word => word.replace(/[^\w]/g, ""))
        .filter(word => word.length > 3);

      const correctAnswer = words[0] || note.title || topic;
      const optionTwo = words[1] || "Revision";
      const optionThree = words[2] || "Practice";
      const optionFour = words[3] || "Learning";

      questions.push({
        question: `(${difficulty}) What key term best represents this ${topic} note?`,
        options: shuffle([
          correctAnswer,
          optionTwo,
          optionThree,
          optionFour
        ]),
        correctAnswer: correctAnswer
      });
    });

    if (questions.length === 0) {
      questions.push({
        question: `(${difficulty}) What topic is this quiz mainly about?`,
        options: shuffle([
          topic,
          "Cooking",
          "Travel",
          "Shopping"
        ]),
        correctAnswer: topic
      });
    }

    return questions;
  }

  function checkAnswer(selectedAnswer, correctAnswer) {
    return {
      isCorrect: selectedAnswer === correctAnswer,
      selectedAnswer: selectedAnswer,
      correctAnswer: correctAnswer
    };
  }

  function shuffle(array) {
    return array.sort(() => Math.random() - 0.5);
  }

  return {
    generateQuestions,
    checkAnswer,
    parseAIQuestions,
    createFallbackQuestions
  };
})();