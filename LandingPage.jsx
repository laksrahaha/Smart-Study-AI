import { useNavigate } from "react-router-dom";

export default function LandingPage() {
  const navigate = useNavigate();

  return (
    <div style={styles.container}>
      
      <nav style={styles.navbar}>
        <img src="/sss-logo.png" alt="SSS Logo" style={styles.navLogo} />
        <span style={styles.navTitle}>Super Smart Study Assistant</span>
      </nav>

      <div style={styles.main}>
        {/* Logo */}
        <img src="/sss-logo.png" alt="SSS Logo" style={styles.logo} />

        <h1 style={styles.title}>SUPER SMART</h1>
        <h2 style={styles.subtitle}>Study Assistant</h2>
        <p style={styles.description}>
          AI-powered study assistant tailored to your course
        </p>

        <div style={styles.buttonRow}>
          <button
            style={styles.button}
            onClick={() => navigate("/login")}
          >
            Login
          </button>
          <span style={styles.or}>OR</span>
          <button
            style={styles.button}
            onClick={() => navigate("/signup")}
          >
            Sign Up
          </button>
        </div>

        <p style={styles.features}>
          Personalised Questions &nbsp;|&nbsp;
          Track Progress &nbsp;|&nbsp;
          AI Feedback
        </p>

        <p style={styles.quote}>
          "It is not the mountain we conquer, but ourselves."
        </p>
        <p style={styles.quoteAuthor}>— Sir Edmund Hillary 🇳🇿</p>
      </div>
    </div>
  );
}

const styles = {
  container: {
    minHeight: "100vh",
    backgroundColor: "#ffffff",
    color: "#101010",
    fontFamily: "'Georgia', serif",
    display: "flex",
    flexDirection: "column",
  },
  navbar: {
    display: "flex",
    alignItems: "center",
    padding: "16px 32px",
    borderBottom: "1px solid #333",
    gap: "12px",
  },
  navLogo: {
    width: "36px",
    height: "36px",
    objectFit: "contain",
  },
  navTitle: {
    fontSize: "18px",
    fontWeight: "bold",
    letterSpacing: "1px",
  },
  main: {
    flex: 1,
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "center",
    padding: "40px 20px",
    textAlign: "center",
  },
  logo: {
    width: "120px",
    height: "120px",
    objectFit: "contain",
    marginBottom: "16px",
  },
  title: {
    fontSize: "48px",
    fontWeight: "bold",
    letterSpacing: "4px",
    margin: "0",
  },
  subtitle: {
    fontSize: "28px",
    fontWeight: "normal",
    margin: "4px 0 12px",
  },
  description: {
    fontSize: "16px",
    color: "#101010",
    marginBottom: "32px",
    fontStyle: "italic",
  },
  buttonRow: {
    display: "flex",
    alignItems: "center",
    gap: "20px",
    marginBottom: "32px",
  },
  button: {
    backgroundColor: "#2d2d2d",
    color: "#ffffff",
    border: "2px solid #ffffff",
    padding: "14px 40px",
    fontSize: "18px",
    fontFamily: "'Georgia', serif",
    cursor: "pointer",
    borderRadius: "8px",
    transition: "all 0.2s ease",
  },
  or: {
    fontSize: "20px",
    color: "#545454",
  },
  features: {
    fontSize: "15px",
    color: "#545454",
    marginBottom: "40px",
  },
  quote: {
    fontSize: "16px",
    fontStyle: "italic",
    color: "#545454",
    maxWidth: "500px",
    marginBottom: "4px",
  },
  quoteAuthor: {
    fontSize: "14px",
    color: "#545454",
  },
};
