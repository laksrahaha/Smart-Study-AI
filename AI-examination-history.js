const AIExaminationHistory = (() => {
  // USER STORY: Examination History
  // This saves quiz attempts so students can review their previous answers.
  // Current sprint scope: localStorage prototype.
  // Future scope: connect this to the existing database using the student/user ID.

  const STORAGE_KEY = "smartStudyAIQuizHistory";

  function saveAttempt({
    topic,
    difficulty,
    question,
    selectedAnswer,
    correctAnswer,
    isCorrect,
    explanation
  }) {
    const history = getHistory();

    const attempt = {
      id: Date.now(),
      topic: topic || "General",
      difficulty: difficulty || "Medium",
      question: question || "",
      selectedAnswer: selectedAnswer || "",
      correctAnswer: correctAnswer || "",
      result: isCorrect ? "Correct" : "Incorrect",
      explanation: explanation || "",
      dateTime: new Date().toLocaleString()
    };

    history.unshift(attempt);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(history));

    return attempt;
  }

  function getHistory() {
    const saved = localStorage.getItem(STORAGE_KEY);

    if (!saved) {
      return [];
    }

    try {
      return JSON.parse(saved);
    } catch (error) {
      console.warn("Could not load quiz history:", error);
      return [];
    }
  }

  function clearHistory() {
    localStorage.removeItem(STORAGE_KEY);
  }

  function renderHistory(containerId) {
    const container = document.getElementById(containerId);

    if (!container) {
      return;
    }

    const history = getHistory();

    if (history.length === 0) {
      container.innerHTML = `
        <p class="empty">No examination history yet. Answer a quiz question to save your first attempt.</p>
      `;
      return;
    }

    container.innerHTML = history.map(item => `
      <div class="history-card">
        <div class="history-header">
          <strong>${escapeHtml(item.result)}</strong>
          <span>${escapeHtml(item.dateTime)}</span>
        </div>

        <p><b>Topic:</b> ${escapeHtml(item.topic)}</p>
        <p><b>Difficulty:</b> ${escapeHtml(item.difficulty)}</p>
        <p><b>Question:</b> ${escapeHtml(item.question)}</p>
        <p><b>Your answer:</b> ${escapeHtml(item.selectedAnswer)}</p>
        <p><b>Correct answer:</b> ${escapeHtml(item.correctAnswer)}</p>

        ${item.explanation
          ? `<p><b>Explanation:</b> ${escapeHtml(item.explanation)}</p>`
          : ""}
      </div>
    `).join("");
  }

  function escapeHtml(str) {
    return String(str || "")
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");
  }

  return {
    saveAttempt,
    getHistory,
    clearHistory,
    renderHistory
  };
})();