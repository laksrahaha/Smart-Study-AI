const AIQuestionGenerator = (() => {

  async function generateQuestions({ topic, difficulty, notes, apiBase }) {
    const noteContent = notes
      .map(note => `Title: ${note.title}\nContent: ${note.content}`)
      .join("\n\n---\n\n");

    const count = 5;
    const difficultyInstruction = AIDifficultyAdjustment.getQuestionDifficultyInstruction(difficulty);

    // FIX 1: Use the same endpoint as the working flash card generator
    // FIX 2: Much stronger prompt that anchors every question to the note content
    const prompt = `
You are an expert quiz generator for students.

Your job is to create ${count} multiple-choice quiz questions based STRICTLY on the study notes provided below.
Every question must be directly answerable from the notes. Do not use outside knowledge or invent facts.

Topic: ${topic}
Difficulty: ${difficulty}
${difficultyInstruction}

--- STUDY NOTES START ---
${noteContent}
--- STUDY NOTES END ---

Rules you MUST follow:
1. Every question must test something explicitly stated in the study notes above.
2. Every question must have exactly 4 answer options.
3. The "correctAnswer" value must be the EXACT text of one of the 4 options — copy it precisely.
4. All 4 options must be plausible — do not make wrong answers obviously silly or unrelated.
5. Questions should vary in style: definitions, cause/effect, comparisons, applications.
6. Do NOT repeat the same question in different wording.
7. Return ONLY a valid JSON array — no markdown, no code fences, no explanation before or after.

Required JSON format:
[
  {
    "question": "Question text here",
    "options": ["Option A", "Option B", "Option C", "Option D"],
    "correctAnswer": "Option A"
  }
]
`;

    try {
      const response = await fetch(`${apiBase}/AI/flashcards`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ content: prompt })
      });

      if (!response.ok) throw new Error(`Server error: ${response.status}`);

      const data = await response.json();

      // Handle both array response and {raw: "..."} response (same as flash card)
      let aiText = "";
      if (Array.isArray(data)) {
        // Backend already parsed it — but it's flashcard format, so fall through to re-prompt
        // This shouldn't happen for quiz, so treat as raw
        aiText = JSON.stringify(data);
      } else if (data.raw) {
        aiText = data.raw;
      } else if (data.result || data.Result) {
        aiText = data.result || data.Result;
      } else {
        aiText = JSON.stringify(data);
      }

      if (!aiText || aiText.includes("Gemini API Error")) {
        throw new Error("AI returned an error response.");
      }

      const parsedQuestions = parseAIQuestions(aiText);

      if (parsedQuestions.length === 0) {
        throw new Error("AI response could not be parsed into quiz questions.");
      }

      return { questions: parsedQuestions, usedFallback: false };

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

      // Strip markdown code fences
      cleaned = cleaned.replace(/^```json\s*/i, "").replace(/^```\s*/i, "").replace(/\s*```$/i, "").trim();

      // Extract the JSON array
      const firstBracket = cleaned.indexOf("[");
      const lastBracket  = cleaned.lastIndexOf("]");

      if (firstBracket !== -1 && lastBracket !== -1) {
        cleaned = cleaned.substring(firstBracket, lastBracket + 1);
      }

      const parsed = JSON.parse(cleaned);

      return parsed
        .filter(q =>
          q.question &&
          Array.isArray(q.options) &&
          q.options.length === 4 &&
          q.correctAnswer &&
          q.options.includes(q.correctAnswer)   // FIX 3: Validate correctAnswer actually matches an option
        )
        .map(q => ({
          question:      q.question,
          options:       q.options,
          correctAnswer: q.correctAnswer
        }));

    } catch (error) {
      console.error("Could not parse AI quiz response:", error);
      return [];
    }
  }

  // FIX 4: Fallback questions that are actually meaningful and educational
  function createFallbackQuestions(topic, difficulty, notes) {
    const questions = [];

    notes.slice(0, 5).forEach(note => {
      if (!note.content || note.content.trim().length < 20) return;

      // Extract meaningful sentences from the note
      const sentences = note.content
        .split(/[.!?]+/)
        .map(s => s.trim())
        .filter(s => s.length > 30 && s.split(" ").length >= 5);

      if (sentences.length === 0) return;

      // Pick a sentence to build a question from
      const sentence = sentences[0];
      const words    = sentence.split(/\s+/).filter(w => w.length > 4);

      if (words.length < 2) return;

      // Pick a key word to blank out as the answer
      const answerWord  = words[Math.floor(words.length / 2)];
      const questionText = sentence.replace(answerWord, "_____");

      // Build 3 plausible distractors from other words in the notes
      const allWords = (note.content || "")
        .split(/\s+/)
        .map(w => w.replace(/[^\w]/g, ""))
        .filter(w => w.length > 4 && w !== answerWord);

      const uniqueWords = [...new Set(allWords)];
      const distractors = uniqueWords.slice(0, 3);

      // Pad distractors if needed
      while (distractors.length < 3) {
        distractors.push(["concept", "process", "method"][distractors.length] || "term");
      }

      const options = shuffle([answerWord, ...distractors.slice(0, 3)]);

      questions.push({
        question:      `(${difficulty}) Fill in the blank from your ${topic} notes: "${questionText}"`,
        options:       options,
        correctAnswer: answerWord
      });
    });

    // Final safety fallback if we still have nothing
    if (questions.length === 0) {
      const note = notes[0] || {};
      questions.push({
        question:      `What is the main subject of the "${note.title || topic}" notes?`,
        options:       shuffle([topic, "Unrelated Subject A", "Unrelated Subject B", "Unrelated Subject C"]),
        correctAnswer: topic
      });
    }

    return questions;
  }

  function checkAnswer(selectedAnswer, correctAnswer) {
    return {
      isCorrect:     selectedAnswer === correctAnswer,
      selectedAnswer,
      correctAnswer
    };
  }

  function shuffle(array) {
    const arr = [...array];
    for (let i = arr.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [arr[i], arr[j]] = [arr[j], arr[i]];
    }
    return arr;
  }

  return {
    generateQuestions,
    checkAnswer,
    parseAIQuestions,
    createFallbackQuestions
  };
})();
